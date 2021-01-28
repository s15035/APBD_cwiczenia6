using cwiczenia3.Models;
using cwiczenia3.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using cwiczenia3.dto.response;
using cwiczenia3.dto.request;
using cwiczenia3.Exceptions;

namespace cwiczenia3.DAL
{
    public class SqlServerDbService : IStudentsDbService
    {
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResponse response;
            DateTime startDate;
            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15035;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();

                var transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                command.CommandText = "SELECT 1 FROM student WHERE IndexNumber=@index";
                command.Parameters.AddWithValue("index", request.IndexNumber);
                var dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    throw new BadRequestException("Such an index already exists.");
                }

                dataReader.Close();

                command.CommandText = "SELECT IdStudy FROM studies WHERE name=@name";
                command.Parameters.AddWithValue("name", request.Studies);
                dataReader = command.ExecuteReader();

                if (!dataReader.Read())
                {
                    dataReader.Close();
                    throw new BadRequestException("There's no such studies.");
                }
                int idStudies = (int)dataReader["IdStudy"];
                dataReader.Close();

                command.CommandText = "SELECT IdEnrollment, StartDate FROM enrollment WHERE IdStudy=@idStudy AND semester=1";
                command.Parameters.AddWithValue("idStudy", idStudies);
                dataReader = command.ExecuteReader();
                int idEnrollment;
                if (!dataReader.Read())
                {
                    dataReader.Close();

                    command.CommandText = "SELECT MAX(\"IdEnrollment\") FROM enrollment";
                    dataReader = command.ExecuteReader();
                    idEnrollment = dataReader.Read() ? (int)dataReader["IdEnrollment"] + 1 : 1;
                    dataReader.Close();
                    startDate = DateTime.Now;
                    command.CommandText = "INSERT INTO enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES(" + "@idEnrollment, 1, @idStudy, @startDate)";
                    command.Parameters.AddWithValue("idEnrollment", idEnrollment);
                    command.Parameters.AddWithValue("idStudy", idStudies);
                    command.Parameters.AddWithValue("startDate", startDate);

                    command.ExecuteNonQuery();
                }
                else
                {
                    idEnrollment = (int)dataReader["IdEnrollment"];
                    startDate = DateTime.Parse(dataReader["StartDate"].ToString());
                    dataReader.Close();
                }
                command.CommandText = "INSERT INTO student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES" + "(@indexNumber, @firstName, @lastName, @birthDate, @idEnrollment)";
                command.Parameters.AddWithValue("indexNumber", request.IndexNumber);
                command.Parameters.AddWithValue("firstName", request.FirstName);
                command.Parameters.AddWithValue("lastName", request.LastName);
                command.Parameters.AddWithValue("birthDate", request.BirthDate);
                command.Parameters.AddWithValue("idEnrollment", idEnrollment);

                command.ExecuteNonQuery();
                transaction.Commit();

                response = new EnrollStudentResponse
                {
                    IdEnrollment = idEnrollment,
                    Semester = 1,
                    StartDate = startDate
                };
            }

            return response;
        }

        public PromotionResponse PromoteStudents(PromotionRequest request)
        {
            PromotionResponse response = new PromotionResponse();
            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15035;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();

                command.CommandText = "SELECT IdStudy FROM studies WHERE name=@name";
                command.Parameters.AddWithValue("name", request.Studies);
                var dataReader = command.ExecuteReader();

                if (!dataReader.Read())
                {
                    throw new BadRequestException("There's no such studies.");
                }
                dataReader.Close();

                command.CommandText = "SELECT 1 FROM Enrollment WHERE semester = @semester AND idStudy = (" +
                    "SELECT IdStudy FROM Studies WHERE Name = @studyName)";
                command.Parameters.AddWithValue("semester", request.Semester);
                command.Parameters.AddWithValue("studyName", request.Studies);
                dataReader = command.ExecuteReader();
                if (!dataReader.Read())
                {
                    throw new NotFoundException("No registration for the given studies and semester.");
                }
                dataReader.Close();

                command.CommandText = "promocja_studenta";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("Studies", request.Studies);
                command.Parameters.AddWithValue("Semester", request.Semester);

                var semesterOut = command.Parameters.Add("@Semester_out", SqlDbType.Int);
                semesterOut.Direction = ParameterDirection.Output;
                var idEnrollmentOut = command.Parameters.Add("@Id_enrollment_out", SqlDbType.Int);
                idEnrollmentOut.Direction = ParameterDirection.Output;
                var startDateOut = command.Parameters.Add("@Start_date_out", SqlDbType.Date);
                startDateOut.Direction = ParameterDirection.Output;
                dataReader = command.ExecuteReader();

                response.IdEnrollment = (int)idEnrollmentOut.Value;
                response.Semester = (int)semesterOut.Value;
                response.StartDate = DateTime.Parse(startDateOut.Value.ToString());

                return response;
            }
        }
        public bool IsValidStudent(string studentIndex)
        {
            using (var connection = new SqlConnection("Data Source=db-mssql;Initial Catalog=s15035;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();

                command.CommandText = "SELECT 1 FROM student WHERE IndexNumber=@index";
                command.Parameters.AddWithValue("studentIndex", studentIndex);
                var dataReader = command.ExecuteReader();
                return dataReader.Read();
            }
        }
    }
}
