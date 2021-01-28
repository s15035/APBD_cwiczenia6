using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cwiczenia3.DAL;
using cwiczenia3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cwiczenia3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        string connectionString = "Data Source=db-mssql;Initial Catalog=s15035;Integrated Security=True";

        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Student> students = new List<Student>();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * FROM Student";

                connection.Open();
                var result = command.ExecuteReader();

                while (result.Read())
                {
                    var student = new Student();
                    student.FirstName = result["FirstName"].ToString();
                    student.LastName = result["LastName"].ToString();
                    student.IndexNumber = result["IndexNumber"].ToString();
                    student.BirthDate = DateTime.Parse(result["BirthDate"].ToString());

                    students.Add(student);
                }
            }
            return Ok(students);
        }

        [HttpGet("{index}")]
        public IActionResult GetStudent(string index)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * FROM Student WHERE IndexNumber=@index";

                command.Parameters.AddWithValue("index", index);

                connection.Open();
                var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    return Ok(st);
                }
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{index}")]
        public IActionResult UpdateStudent(string index)
        {
            return Ok("Student updated.");
        }

        [HttpDelete("{index}")]
        public IActionResult RemoveStudent(string index)
        {
            return Ok("Student deleted.");
        }
    }
}
