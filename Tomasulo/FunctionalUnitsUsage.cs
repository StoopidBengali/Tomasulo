﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Tomasulo
{
    class FuUsage
    {
        #region Membres
        private static string Instruction;
        private static string InstructionName;
        static DataTable resourcesDT;
        #endregion

        #region Properties
        public static DataTable FUUsageDT()
        {
            return resourcesDT;
        } 
        #endregion

        #region Ctor
        public FuUsage()
        {
            LoadDefaults();
        }


        #endregion

        #region Methods
        public static void RefreshFUnits()
        {
            resourcesDT.Clear();
        }

        public static bool Insert(int numOfIterations, int clockCycle)
        {
            resourcesDT.Rows.Add(clockCycle);
            return true;
        }

        public static bool Update(int iteration, int clockCycle, int instructionNum) // method to figure the istruction type and their respective clock cycles
        {
            UpdateCDB(clockCycle, instructionNum);
            InstructionName = InstructionSet.GetInstruction(InstructionFromInput.InstructionsFromInputDT().Rows[instructionNum]["Instruction Name"].ToString());
            switch (InstructionName)
            {
                case "FP Add":
                    UpdtFloatingPointAdd(clockCycle, instructionNum);
                    break;
                case "FP Multiply":
                    UpdateFPMULT(clockCycle, instructionNum);
                    break;
                case "Integer":
                    UpdateINT(clockCycle, instructionNum);
                    break;
                case "Branch":
                    UpdateINT(clockCycle, instructionNum);
                    break;
                case "LD/SD":
                    UpdateLD(clockCycle, instructionNum);
                    break;
                default:
                    break;
            }
            return true;
        }

        private static void UpdateCDB(int clockCycle, int instructionNum) // method to look at CDB and find out the respective ones
        {
            Instruction = InstructionNameConstruct(instructionNum);
           
            if (InstructionStatusManager.GetCDBCycle(instructionNum) == clockCycle)
            {
                if (string.Compare(resourcesDT.Rows[clockCycle - 1]["CDB"].ToString(), string.Empty) == 0)
                {
                    resourcesDT.Rows[clockCycle - 1]["CDB"] = Instruction + ", ";
                }
                else
                {
                    resourcesDT.Rows[clockCycle - 1]["CDB"] += Instruction + ", ";
                }
            }
        }

        private static void UpdtFloatingPointAdd(int clockCycle, int instructionNum)
        {
            Instruction = InstructionNameConstruct(instructionNum);
            if (InstructionStatusManager.ExecutionCycle(instructionNum) == clockCycle)
            {
                if (string.Compare(resourcesDT.Rows[clockCycle - 1]["FP Add"].ToString(), string.Empty) == 0)
                {
                    resourcesDT.Rows[clockCycle - 1]["FP Add"] = Instruction + ", ";
                }
                else
                {
                    resourcesDT.Rows[clockCycle - 1]["FP Add"] += Instruction + ", ";
                }
            }

        }

        private static void UpdateFPMULT(int clockCycle, int instructionNum)
        {
            Instruction = InstructionNameConstruct(instructionNum);
            if (InstructionStatusManager.ExecutionCycle(instructionNum) == clockCycle)
            {
                if (string.Compare(resourcesDT.Rows[clockCycle - 1]["FP Mult"].ToString(), string.Empty) == 0)
                {
                    resourcesDT.Rows[clockCycle - 1]["FP Mult"] = Instruction + ", ";
                }
                else
                {
                    resourcesDT.Rows[clockCycle - 1]["FP Mult"] += Instruction + ", ";
                }
            }

        }

        private static void UpdateINT(int clockCycle, int instructionNum)
        {
            Instruction = InstructionNameConstruct(instructionNum);
            if (InstructionStatusManager.ExecutionCycle(instructionNum) == clockCycle)
            {
                if (string.Compare(resourcesDT.Rows[clockCycle - 1]["Integer ALU"].ToString(), string.Empty) == 0)
                {
                    resourcesDT.Rows[clockCycle - 1]["Integer ALU"] = Instruction + ", ";
                }
                else
                {
                    resourcesDT.Rows[clockCycle - 1]["Integer ALU"] += Instruction + ", ";
                }
            }

        }

        private static void UpdateLD(int clockCycle, int instructionNum)
        {
            if (Instruction == "LD" || Instruction == "L.D")
            {
                if (InstructionStatusManager.MemoryReadCycle(instructionNum) == clockCycle)
                {
                    FUCycleUpdate(clockCycle, instructionNum);
                }
            }
            else
            {
                if (InstructionStatusManager.CommitCycle(instructionNum) == clockCycle)
                {
                    FUCycleUpdate(clockCycle, instructionNum);
                }
            }

        }
        /*--------------------------------------------------------------------------------
         Function name   : InstructionNameConstruct        
         Description     : method to reconstruct the complete istruction from the memory
         Return type     : string 
         Argument        : int instIndex
        ---------------------------------------------------------------------------------*/
        private static string InstructionNameConstruct(int instIndex)
        {
            return InstructionFromInput.InstructionsFromInputDT().Rows[instIndex]["Instruction Name"].ToString()
                + " " + InstructionFromInput.InstructionsFromInputDT().Rows[instIndex]["DestReg"].ToString()
                + ", " + InstructionFromInput.InstructionsFromInputDT().Rows[instIndex]["SourceJ"].ToString()
                + ", "+ InstructionFromInput.InstructionsFromInputDT().Rows[instIndex]["SourceK"].ToString();
        }

        /*--------------------------------------------------------------------------------
         Function name   : InstructionNameConstruct        
         Description     : method to initialize the member variables with default values
         Return type     : void
        ---------------------------------------------------------------------------------*/


        private static void LoadDefaults()
        {
            resourcesDT = new DataTable("Resources");
            resourcesDT.Columns.Add("Cycle", typeof(int));
            resourcesDT.Columns.Add("Integer ALU", typeof(string));
            resourcesDT.Columns.Add("FP Add", typeof(string));
            resourcesDT.Columns.Add("FP Mult", typeof(string));
            resourcesDT.Columns.Add("Data Cache", typeof(string));
            resourcesDT.Columns.Add("CDB", typeof(string));
            Instruction = string.Empty;
            InstructionName = string.Empty;
        }


        /*---------------------------------------------------------------------------------------------------
         Function name   : FUCycleUpdate       
         Description     : method to update the status of the FU as the instructions keep on getting executed.
         Return type     : string 
         Argument        : int instIndex
         --------------------------------------------------------------------------------------------------*/

        private static void FUCycleUpdate(int clockCycle, int instructionNum)
        {
            Instruction = InstructionNameConstruct(instructionNum);
            if (string.Compare(resourcesDT.Rows[clockCycle - 1]["Data Cache"].ToString(), string.Empty) == 0)
            {
                resourcesDT.Rows[clockCycle - 1]["Data Cache"] = Instruction + ", ";
            }
            else
            {
                resourcesDT.Rows[clockCycle - 1]["Data Cache"] += Instruction + ", ";
            }
        } 
        #endregion
    }
}
