using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Agava.MenuExtender
{
    [HarmonyPatch]
    internal class MenuPath
    {
        private static List<CodeInstruction> OverrideInstructions = new();
        private static Type DeclaringType;

        private static readonly Dictionary<OpCode, Func<CodeInstruction, int>> Stlocs = new()
        {
            { OpCodes.Stloc_0, (instruction) => 0},
            { OpCodes.Stloc_1, (instruction) => 1},
            { OpCodes.Stloc_2, (instruction) => 2},
            { OpCodes.Stloc_3, (instruction) => 3},
            { OpCodes.Stloc_S, (instruction) => ((LocalBuilder)(instruction.operand)).LocalIndex},
            { OpCodes.Stloc, (instruction) => ((LocalBuilder)(instruction.operand)).LocalIndex},
        };

        private static readonly Dictionary<OpCode, Func<CodeInstruction, int>> Ldlocs = new()
        {
            { OpCodes.Ldloc_0, (instruction) => 0},
            { OpCodes.Ldloc_1, (instruction) => 1},
            { OpCodes.Ldloc_2, (instruction) => 2},
            { OpCodes.Ldloc_3, (instruction) => 3},
            { OpCodes.Ldloc_S, (instruction) => ((LocalBuilder)(instruction.operand)).LocalIndex},
            { OpCodes.Ldloc, (instruction) => ((LocalBuilder)(instruction.operand)).LocalIndex},
        };

        private const string VirtualMethodName = "ConstructMenu";

        private static IEnumerable<MethodBase> TargetMethods()
        {
            Type[] types = MenuExtender.DerivedTypes;
            var methods = new List<MethodBase>();

            foreach (var type in types)
            {
                object[] attributes = type.GetCustomAttributes(false);
                foreach (Attribute attr in attributes)
                {
                    if (attr is MenuWindowAttribute ageAttribute)
                    {
                        methods.Add(AccessTools.Method(type, VirtualMethodName));
                        methods.Add(ageAttribute.Window.Method());
                    }
                }
            }

            return methods;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr, ILGenerator iLGenerator, MethodBase methodBase)
        {
            if (methodBase.Name == VirtualMethodName)
            {
                OverrideInstructions = new List<CodeInstruction>(instr);
                DeclaringType = methodBase.DeclaringType;
                return instr;
            }

            var instructions = new List<CodeInstruction>();

            MethodInfo methodINfo = DeclaringType.GetMethod(VirtualMethodName);
            MethodBody methodBody = methodINfo.GetMethodBody();

            var variables = new Dictionary<int, LocalBuilder>();
            var labels = new Dictionary<Label, Label>();

            for (int i = 0; i < OverrideInstructions.Count; i++)
            {
                OverrideInstructions[i] = SetupLabels(OverrideInstructions[i], ref labels, iLGenerator);
                OverrideInstructions[i] = SetupStloc(OverrideInstructions[i], ref variables, methodBody.LocalVariables, iLGenerator);
                OverrideInstructions[i] = SetupLdloc(OverrideInstructions[i], ref variables);
            }

            if (OverrideInstructions.Count > 0)
                OverrideInstructions[^1].opcode = OpCodes.Nop;

            instructions.AddRange(OverrideInstructions);
            instructions.AddRange(instr);

            return instructions;
        }

        private static CodeInstruction SetupLabels(CodeInstruction instruction, ref Dictionary<Label, Label> labels, ILGenerator iLGenerator)
        {
            for (int i = 0; i < instruction.labels.Count; i++)
            {
                var key = instruction.labels[i];

                if (labels.TryGetValue(key, out Label label) == false)
                    labels.Add(key, label = iLGenerator.DefineLabel());

                instruction.labels[i] = label;
            }

            if (instruction.operand is Label)
            {
                var key = (Label)instruction.operand;

                if (labels.TryGetValue(key, out Label label) == false)
                    labels.Add(key, label = iLGenerator.DefineLabel());

                instruction.operand = label;
            }

            return instruction;
        }

        private static CodeInstruction SetupStloc(CodeInstruction instruction, ref Dictionary<int, LocalBuilder> variables, IList<LocalVariableInfo> localVariables, ILGenerator iLGenerator)
        {
            if (IsStloc(instruction.opcode))
            {
                var index = StlocIndex(instruction);
                var localType = localVariables.First(v => v.LocalIndex == index).LocalType;

                if (variables.ContainsKey(index) == false)
                    variables.Add(index, iLGenerator.DeclareLocal(localType));

                instruction.opcode = OpCodes.Stloc;
                instruction.operand = variables[index];
            }

            return instruction;
        }

        private static CodeInstruction SetupLdloc(CodeInstruction instruction, ref Dictionary<int, LocalBuilder> variables)
        {
            if (IsLdloc(instruction.opcode))
            {
                var variableIndex = LdlocIndex(instruction);
                instruction.operand = variables[variableIndex];
                instruction.opcode = OpCodes.Ldloc;
            }

            return instruction;
        }

        private static bool IsStloc(OpCode opCode)
        {
            return Stlocs.ContainsKey(opCode);
        }

        private static int StlocIndex(CodeInstruction instruction)
        {
            return Stlocs[instruction.opcode](instruction);
        }

        private static bool IsLdloc(OpCode opCode)
        {
            return Ldlocs.ContainsKey(opCode);
        }

        private static int LdlocIndex(CodeInstruction instruction)
        {
            return Ldlocs[instruction.opcode](instruction);
        }
    }
}
