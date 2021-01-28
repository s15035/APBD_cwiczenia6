using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cwiczenia3.dto.response
{
    public class EnrollStudentResponse
    {
        public int IdEnrollment { get; set; }

        public int Semester { get; set; }

        public DateTime StartDate { get; set; }

    }
}
