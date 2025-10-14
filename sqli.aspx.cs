using System;
using System.Data;
using System.Data.SqlClient;

namespace CxCE_Demo
{
    public partial class sqli : System.Web.UI.Page
    {
        static string username = String.Empty;
        static int age = -1;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string ID = Request.QueryString[0];
                getName(ID);
            }
            catch { }
        }

        private void getName(string ID)
        {
           string username = "No name";

           string connectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=" + Constants.DB_PASSWORD + ";";
    
           using (SqlConnection conn = new SqlConnection(connectionString))
           {
               using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT NAME FROM Users WHERE ID = @IDParam";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;
                    if (int.TryParse(ID, out int userId))
                    {
                        cmd.Parameters.Add("@IDParam", SqlDbType.Int).Value = userId;
                    }
                    else
                    {
                        message.Text = "Error: Invalid ID format.";
                    }

                    conn.Open();

                    // Use 'using' for proper resource disposal (SqlDataReader)
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // Use .Read() instead of .HasRows and then .Read()
                        {
                            username = reader["NAME"].ToString();
                            age = getAge(username);
                        }
                    } // reader is closed and disposed here
                } // cmd is disposed here
            } // conn is closed and disposed here

            string encodedUsername = System.Net.WebUtility.HtmlEncode(username);
            message.Text = "Welcome " + encodedUsername;
        }

        private int getAge(string name)
        {
            SqlConnection conn = new SqlConnection("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=" + Constants.DB_PASSWORD + ";");
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "SELECT AGE FROM Users WHERE NAME = '" + name + "'";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            conn.Open();

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
               return(reader.GetInt32(0));

            conn.Close();
            return -1;
        }

        protected void submit_Click(object sender, EventArgs e)
        {
            getName(name.Text);
        }
    }
}
