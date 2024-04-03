using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public static class ValidateConfig
    {
        public static readonly Dictionary<string, object> InputValidation = new Dictionary<string, object>()
        {
            {
                "CapstoneGPA",
                new Dictionary<string, object>()
                {
                    {
                        "Group", new Dictionary<string, object>()
                        {
                            { "required", true },
                            { "maxLength", 10 },
                            { "minLength", 1 },
                        }
                    },
                    {
                        "GroupMajor", new Dictionary<string, object>()
                        {
                            { "required", true },
                            { "maxLength", 10 },
                            { "minLength", 1 },
                        }
                    },
                    {
                        "GroupMember", new Dictionary<string, object>()
                        {
                            { "required", true },
                            { "maxLength", 10 },
                            { "minLength", 1 },
                        }
                    },
                    {
                        "MemberMajor", new Dictionary<string, object>()
                        {
                            { "required", true },
                            { "maxLength", 10 },
                            { "minLength", 1 },
                        }
                    },
                    {
                        "Gpa", new Dictionary<string, object>()
                        {
                            { "required", true },
                            { "GPAValueOutOfRange", true },
                        }
                    },
                    {
                        "Semester_no", new Dictionary<string, object>()
                        {
                            { "required", true },
                            { "maxLength", 1 },
                            { "minLength", 1 },
                        }
                    },
                    {
                        "Semester", new Dictionary<string, object>()
                        {
                            { "required", true },
                            { "maxLength", 10 },
                            { "minLength", 1 },
                        }
                    },
                    {
                        "Is_Capstone", new Dictionary<string, object>()
                        {
                            { "required", true },
                            { "pattern", "TRUE|FALSE" },
                        }
                    },
                    {
                        "SemesterCredit", new Dictionary<string, object>()
                        {
                            { "required", true },
                            { "maxLength", 10 },
                            { "minLength", 1 },
                        }
                    },
                    {
                        "Year", new Dictionary<string, object>()
                        {
                            { "required", true },
                            { "format", "yyyy" },
                        }
                    },
                }
            },
        };
        public static readonly Dictionary<string, object> InputValidationNumber = new Dictionary<string, object>()
        {
            {
                "CapstoneGPA",
                new Dictionary<string, object>()
                {
                    { "0", "Group" },
                    { "1", "GroupMajor" },
                    { "2", "GroupMember" },
                    { "3", "MemberMajor" },
                    { "4", "Gpa" },
                    { "5", "Semester_no" },
                    { "6", "Semester" },
                    { "7", "Is_Capstone" },
                    { "8", "SemesterCredit" },
                    { "9", "Year" }
                }
            }
        };
        public static readonly Dictionary<string, object> ToolAvailable = new Dictionary<string, object>()
        {
            {
                "CapstoneGPA",
                new List<string>()
                {
                    {"GPA Capstone"}
                }
            }
        };
    }
}
