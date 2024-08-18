﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static CW.PersonDAL;

namespace CW
{
    public class PersonDAL
    {
        static void Main(string[] args)
        {
            // Example usage of PersonDAL
            PersonDAL personDAL = new PersonDAL();
            DataTable allPersons = personDAL.Read();
            Console.WriteLine("All persons data loaded.");
        }

        public string ConString = "Data Source=ADMIN-PC\\SQLEXPRESS;Initial Catalog=EducationCentre;Persist Security Info=True;User ID=sa;Password=zxc;TrustServerCertificate=True";

        SqlConnection con = new SqlConnection();
        DataTable dt = new DataTable();

        public DataTable Read()
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Person ORDER BY PersonId DESC", con);
            try
            {
                SqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public DataTable Read(Int16 id)
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();
            SqlCommand cmd = new SqlCommand("select * from Person where PersonId= " + id, con);
            try
            {
                SqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public DataTable ReadByName(String Name)
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();

            // Use the LIKE operator with a parameterized query
            SqlCommand cmd = new SqlCommand("SELECT * FROM Person WHERE Name LIKE @Name", con);

            // Add the Name parameter and use wildcard characters if needed
            cmd.Parameters.AddWithValue("@Name", "%" + Name + "%");

            try
            {
                SqlDataReader rd = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rd);
                return dt;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error reading person data by name with LIKE.", ex);
            }
        }

        public DataTable ReadTeachers()
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();

            // Modify the SQL query to join Teacher with Person
            string query = @"
                SELECT 
                    Teacher.TeacherId,
                    Teacher.PersonId,
                    Person.Name,
                    Person.Telephone,
                    Person.Email,
                    Person.Role,
                    Teacher.Salary,
                    Teacher.Subject1,
                    Teacher.Subject2
                FROM 
                    Teacher
                INNER JOIN 
                    Person ON Teacher.PersonId = Person.PersonId
                ORDER BY PersonId DESC";

            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                SqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public DataTable ReadPersonInfo(object personId, object personRole)
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
            {
                con.Open();
            }

            // Validate and sanitize the personRole parameter
            string[] allowedRoles = { "Teacher", "Admin", "Student" };
            if (!allowedRoles.Contains(personRole.ToString()))
            {
                throw new ArgumentException("Invalid role specified.");
            }

            // Construct the SQL query string dynamically with table aliases
            string query = $"SELECT * FROM [{personRole}] AS RoleTable " +
                           $"INNER JOIN Person AS P ON RoleTable.PersonId = P.PersonId " +
                           $"WHERE P.PersonId = @PersonId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@PersonId", personId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    dt = new DataTable();
                    da.Fill(dt);
                }
            }

            con.Close();
            return dt;
        }

        public DataTable ReadAdmins()
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();

            // Modify the SQL query to join Admin with Person
            string query = @"
                SELECT 
                    Admin.AdminId,
                    Admin.PersonId,
                    Person.Name,
                    Person.Telephone,
                    Person.Email,
                    Person.Role,
                    Admin.Salary,
                    Admin.EmploymentType,
                    Admin.WorkingHours
                FROM 
                    Admin
                INNER JOIN 
                    Person ON Admin.PersonId = Person.PersonId
                ORDER BY PersonId DESC";

            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                SqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public DataTable ReadStudents()
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();

            // Modify the SQL query to join Student with Person
            string query = @"
                SELECT 
                    Student.StudentId,
                    Student.PersonId,
                    Person.Name,
                    Person.Telephone,
                    Person.Email,
                    Person.Role,
                    Student.CurrentSubject1,
                    Student.CurrentSubject2,
                    Student.PreviousSubject1,
                    Student.PreviousSubject2
                FROM 
                    Student
                INNER JOIN 
                    Person ON Student.PersonId = Person.PersonId
                ORDER BY PersonId DESC";

            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                SqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public int CreatePerson(string name, string telephone, string email, string role)
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();

            string query = "INSERT INTO Person (Name, Telephone, Email, Role) OUTPUT INSERTED.PersonId VALUES (@Name, @Telephone, @Email, @Role)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Telephone", telephone);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Role", role);

            try
            {
                int personId = (int)cmd.ExecuteScalar();
                return personId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating person", ex);
            }
            finally
            {
                con.Close();
            }
        }

        

        


        public void CreateTeacher(int personId, decimal salary, string subject1, string subject2)
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();

            string query = "INSERT INTO Teacher (PersonId, Salary, Subject1, Subject2) VALUES (@PersonId, @Salary, @Subject1, @Subject2)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@PersonId", personId);
            cmd.Parameters.AddWithValue("@Salary", salary);
            cmd.Parameters.AddWithValue("@Subject1", subject1);
            cmd.Parameters.AddWithValue("@Subject2", subject2);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating teacher", ex);
            }
            finally
            {
                con.Close();
            }
        }

        public int UpdatePerson(int personId, string name = null, string telephone = null, string email = null, string role = null)
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();

            List<string> updateFields = new List<string>();
            if (!string.IsNullOrEmpty(name)) updateFields.Add("Name = @Name");
            if (!string.IsNullOrEmpty(telephone)) updateFields.Add("Telephone = @Telephone");
            if (!string.IsNullOrEmpty(email)) updateFields.Add("Email = @Email");
            if (!string.IsNullOrEmpty(role)) updateFields.Add("Role = @Role");

            if (updateFields.Count == 0)
            {
                throw new Exception("No fields to update");
            }

            string query = $"UPDATE Person SET {string.Join(", ", updateFields)} WHERE PersonId = @PersonId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@PersonId", personId);
            if (!string.IsNullOrEmpty(name)) cmd.Parameters.AddWithValue("@Name", name);
            if (!string.IsNullOrEmpty(telephone)) cmd.Parameters.AddWithValue("@Telephone", telephone);
            if (!string.IsNullOrEmpty(email)) cmd.Parameters.AddWithValue("@Email", email);
            if (!string.IsNullOrEmpty(role)) cmd.Parameters.AddWithValue("@Role", role);

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating person", ex);
            }
            finally
            {
                con.Close();
            }
        }



        public void UpdateTeacher(int personId, decimal? salary = null, string subject1 = null, string subject2 = null)
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();

            List<string> updateFields = new List<string>();
            if (salary.HasValue) updateFields.Add("Salary = @Salary");
            if (!string.IsNullOrEmpty(subject1)) updateFields.Add("Subject1 = @Subject1");
            if (!string.IsNullOrEmpty(subject2)) updateFields.Add("Subject2 = @Subject2");

            if (updateFields.Count == 0)
            {
                throw new Exception("No fields to update");
            }

            string query = $"UPDATE Teacher SET {string.Join(", ", updateFields)} WHERE PersonId = @PersonId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@PersonId", personId);
            if (salary.HasValue) cmd.Parameters.AddWithValue("@Salary", salary.Value);
            if (!string.IsNullOrEmpty(subject1)) cmd.Parameters.AddWithValue("@Subject1", subject1);
            if (!string.IsNullOrEmpty(subject2)) cmd.Parameters.AddWithValue("@Subject2", subject2);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating teacher: " + ex.Message, ex);
            }
            finally
            {
                con.Close();
            }
        }





        public void CreateAdmin(int personId, decimal salary, string employmenttype, decimal workinghours)
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();

            string query = "INSERT INTO Admin (PersonId, Salary, EmploymentType, WorkingHours) VALUES (@PersonId, @Salary, @EmploymentType, @WorkingHours)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@PersonId", personId);
            cmd.Parameters.AddWithValue("@Salary", salary);
            cmd.Parameters.AddWithValue("@EmploymentType", employmenttype);
            cmd.Parameters.AddWithValue("@WorkingHours", workinghours);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating Admin", ex);
            }
            finally
            {
                con.Close();
            }
        }

        public void CreateStudent(int personId, string currentsubject1, string currentsubject2, string previoussubject1, string previoussubject2)
        {
            con.ConnectionString = ConString;
            if (ConnectionState.Closed == con.State)
                con.Open();

            string query = "INSERT INTO Student (PersonId, CurrentSubject1, CurrentSubject2, PreviousSubject1, PreviousSubject2) VALUES (@PersonId, @CurrentSubject1, @CurrentSubject2, @PreviousSubject1, @PreviousSubject2)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@PersonId", personId);
            cmd.Parameters.AddWithValue("@CurrentSubject1", currentsubject1);
            cmd.Parameters.AddWithValue("@CurrentSubject2", currentsubject2);
            cmd.Parameters.AddWithValue("@PreviousSubject1", previoussubject1);
            cmd.Parameters.AddWithValue("@PreviousSubject2", previoussubject2);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating Student", ex);
            }
            finally
            {
                con.Close();
            }
        }

        // Method to delete a person
        public void Delete(Person person)
        {
            using (SqlConnection con = new SqlConnection(ConString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Person WHERE Name = @Name", con);
                cmd.Parameters.AddWithValue("@Name", person.Name);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting data", ex);
                }
            }
        }

        public class Person
        {
            public string Name { get; private set; }
            public string Telephone { get; set; }

            public string Email { get; set; }
            public string Role { get; set; }

            public virtual void DisplayInfo()
            {
                Console.WriteLine($"Name: {Name}, Telephone: {Telephone}, Email: {Email}, Role: {Role}");
            }
        }

        public class Teacher : Person
        {
            public float Salary { get; set; }
            public string Subject1 { get; set; }
            public string Subject2 { get; set; }

            public override void DisplayInfo()
            {
                base.DisplayInfo();
                Console.WriteLine($"Salary: {Salary}, Subject1: {Subject1}, Subject2: {Subject2}");
            }
        }

        public class Admin : Person
        {
            public float Salary { get; set; }
            public string EmploymentType { get; set; }
            public int WorkingHours { get; set; }

            public override void DisplayInfo()
            {
                base.DisplayInfo();
                Console.WriteLine($"Salary: {Salary}, EmploymentType: {EmploymentType}, WorkingHours: {WorkingHours}");
            }
        }

        public class Student : Person
        {
            public string CurrentSubject1 { get; set; }
            public string CurrentSubject2 { get; set; }
            public string PreviousSubject1 { get; set; }
            public string PreviousSubject2 { get; set; }

            public override void DisplayInfo()
            {
                base.DisplayInfo();
                Console.WriteLine($"CurrentSubject1: {CurrentSubject1}, CurrentSubject2: {CurrentSubject2}, PreviousSubject1: {PreviousSubject1}, PreviousSubject2: {PreviousSubject2}");
            }
        }
    }
}
