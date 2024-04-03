using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class GradesGPA
    {
        public string Group {  get; set; }
        public string GroupMajor {  get; set; }
        public string GroupMember {  get; set; }
        public string MemberMajor {  get; set; }
        public double Gpa {  get; set; }
        public int Semester_no {  get; set; }
        public string Semester {  get; set; }
        public bool Is_Capstone {  get; set; }
        public int SemesterCredit {  get; set; }
        public int Year {  get; set; }
    }
}
