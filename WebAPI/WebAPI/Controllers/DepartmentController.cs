using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _sqlDataSource;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
        }

        [HttpGet] 
        public JsonResult Get()
        {
            string query = @"select DepartmentId, DepartmentName from dbo.Department";
            DataTable table = ConnectToDBAndExecuteQuery(query);
            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Department department)
        {
            string query = @"insert into dbo.Department values ('" + department.DepartmentName + @"') ";
            ConnectToDBAndExecuteQuery(query);
            return new JsonResult("New Department Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(Department department)
        {
            string query = @"update dbo.Department set DepartmentName = '" + department.DepartmentName  + @"' 
                             where DepartmentId = " + department.DepartmentId  +@"";

            ConnectToDBAndExecuteQuery(query);
            return new JsonResult("Department Updated Successfully");
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"delete from dbo.Department where DepartmentId = " + id + @"";
            ConnectToDBAndExecuteQuery(query);
            return new JsonResult("Department deleted Successfully");
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
