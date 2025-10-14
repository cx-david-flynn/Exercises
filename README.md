# SQL Injection fix to the sqli.aspx.cs file<br>
Source of solution: https://g.co/gemini/share/0acc86899abc

```
private void getName(string ID)
{
    // Use a variable to hold the final username
    string username = "No name";

    // 1. Use 'using' for proper resource disposal (SqlConnection)
    // NEVER put sensitive information like the password in code. Use app settings/secrets.
    string connectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=" + Constants.DB_PASSWORD + ";";
    
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        // 2. Use 'using' for proper resource disposal (SqlCommand)
        using (SqlCommand cmd = new SqlCommand())
        {
            // 3. FIX: Use a parameterized query to prevent SQL Injection
            cmd.CommandText = "SELECT NAME FROM Users WHERE ID = @IDParam";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            // Add the parameter and its value, ensuring it's treated as data, not code.
            // Assuming ID is an integer, use Add instead of AddWithValue for type safety.
            if (int.TryParse(ID, out int userId))
            {
                cmd.Parameters.Add("@IDParam", SqlDbType.Int).Value = userId;
            }
            else
            {
                // Handle invalid input, log error, or throw exception
                message.Text = "Error: Invalid ID format.";
                return; 
            }

            conn.Open();

            // Use 'using' for proper resource disposal (SqlDataReader)
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read()) // Use .Read() instead of .HasRows and then .Read()
                {
                    // No fix needed here, but it's important not to trust this data 
                    // until it's encoded on output (see point 4).
                    username = reader["NAME"].ToString();
                    age = getAge(username);
                }
            } // reader is closed and disposed here
        } // cmd is disposed here
    } // conn is closed and disposed here

    // 4. FIX: Use encoding when outputting user data to prevent XSS
    // Note: If this is an ASP.NET page, the 'message.Text' assignment 
    // might be inherently safe (HTML-encoded) by the framework. 
    // This explicit encoding is a safety layer for any string output.
    string encodedUsername = System.Net.WebUtility.HtmlEncode(username);
    message.Text = "Welcome " + encodedUsername;
}
```
