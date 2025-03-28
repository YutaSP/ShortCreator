import pyodbc

def query_top_1_record(connection_string, table_name):
    # Establish connection to the SQL Server database
    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()

    # Construct the query to select the top 1 record
    query = f"""
    SELECT TOP 1 * 
    FROM {table_name}
    """

    # Execute the query with the provided parameters (search_value and id_value)
    cursor.execute(query)

    # Fetch the result
    result = cursor.fetchone()

    # Check if a result was returned
    if result:
        return result
    else:
        return None
    

connection_string = 'DRIVER={ODBC Driver 17 for SQL Server};SERVER=localhost,1433;DATABASE=Short_Maker_Prod;UID=sa;PWD=!Test123'

# Define the table and columns
table_name = 'Reddit_Stories'
result = query_top_1_record(connection_string, table_name)
print(result[0])
