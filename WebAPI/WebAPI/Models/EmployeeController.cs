using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using WebAPI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly string _sqlDataSource;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select EmployeeId, EmployeeName, Department,
                             convert(varchar(10), DateOfJoining, 120) as DateOfJoining, PhotoFileName from dbo.Employee";
            DataTable table = ConnectToDBAndExecuteQuery(query); 
            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Employee employee)
        {
            string query = @"insert into dbo.Employee (EmployeeName,Department,DateOfJoining,PhotoFileName)
            values('" + employee.EmployeeName + @"', '" + employee.Department + @"', 
                   '" + employee.DateOfJoining + @"', '" + employee.PhotoFileName + @"')";

            ConnectToDBAndExecuteQuery(query);
            return new JsonResult("Employee Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(Employee employee)
        {
            string query = @"update dbo.Employee set
            EmployeeName = '" + employee.EmployeeName + @"', Department = '" + employee.Department + @"',
            DateOfJoining = '" + employee.DateOfJoining + @"', PhotoFileName = '" + employee.PhotoFileName + @"',    
            where EmployeeId = " + employee.EmployeeId + @"";
            ConnectToDBAndExecuteQuery(query);
            return new JsonResult("Employee Updated Successfully");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {

                return new JsonResult("anonymous.png");
            }
        }


        [Route("GetAllDepartmentNames")]
        public JsonResult GetAllDepartmentNames()
        {
            string query = @"select DepartmentName from dbo.Department ";
            DataTable table = ConnectToDBAndExecuteQuery(query);
            return new JsonResult(table);
        }


        [HttpDelete("{id}")]
        
        public JsonResult Delete(int id)

        {
            string query = @"delete from dbo.Employee where EmployeeId = " + id + @"";
            ConnectToDBAndExecuteQuery(query);
            return new JsonResult("Employee deleted Successfully");
        }



        public DataTable ConnectToDBAndExecuteQuery(string query)
        {
            DataTable table = new DataTable();
            SqlDataReader sqlDataReader;
            using (SqlConnection connection = new SqlConnection(_sqlDataSource))
            {
                connection.Open();

                using SqlCommand sqlCommand = new SqlCommand(query, connection);
                sqlDataReader = sqlCommand.ExecuteReader();
                table.Load(sqlDataReader); ;

                sqlDataReader.Close();
                connection.Close();
            }

            return table;
        }
    }
}
