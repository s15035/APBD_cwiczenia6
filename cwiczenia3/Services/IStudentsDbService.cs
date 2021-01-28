using cwiczenia3.dto.request;
using cwiczenia3.dto.response;
using cwiczenia3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cwiczenia3.DAL
{
    public interface IStudentsDbService
    {
        PromotionResponse PromoteStudents(PromotionRequest request);

        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);

        bool IsValidStudent(string studentIndex);
    }
}
