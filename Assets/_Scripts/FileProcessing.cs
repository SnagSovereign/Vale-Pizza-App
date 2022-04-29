using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileProcessing : MonoBehaviour
{

    string source; //where the files will come from
    string destPath; //will be the save location for downloaded files
    string destFile; //will create / hold the local location of the csv file

    string[] records; //will store each line of the csv file in one array element
    string[] values; //will store the values after they are broken from the record line
    string dbPath; //will hold the path to the string in the streaming assets folder

    void DropTable(string fileName)
    {
        //Setup the query
        string sql = "DROP TABLE IF EXISTS " + fileName;

        //run the query
        RunMyQuery(sql);
    }

    void CreateTable(string fileName)
    {
        //drop the .csv from the filename variable, convert it to lowercase and save it back to itself
        fileName = fileName.Substring(0, (fileName.Length - 4)).ToLower();

        //Drop the table in case it exists already
        DropTable(fileName);

        //Setup the query
        string sql = "CREATE TABLE " + fileName +
                     "( ID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                     values[0] + " TEXT, " +
                     values[1] + " TEXT, " +
                     values[2] + " TEXT, " +
                     values[3] + " INTEGER, " +
                     values[4] + " REAL, " +
                     values[5] + " TEXT);";

        //run the query
        RunMyQuery(sql);
    }

    void RunMyQuery(string query)
    {
        //this method is used to handle the DB.cs file and DB interactions for connecting and running queries
        //It is designed to save having to write multiple instances of connection code and simplify the logic

        //set the path to the DB file / Create the file in the path we want
        dbPath = "URI=file:" + Path.Combine(Application.streamingAssetsPath, "ValePizza.db");
        //Connect to the DB
        DB.Connect(dbPath);

        //run the query
        DB.RunQuery(query);

        //Close the DB
        DB.CloseDB();
    }

    string[] BreakIntoValues(string line)
    {
        int valueIndex = 0; //stores the index of the value
        int lastCommaIndex = 0; //will store the location of the start of the next substring
        bool inQuote = false;

        //loop through the string looking for commas
        for (int i = 0; i < line.Length; i++)
        {
            if(line[i] == '"')
            {
                inQuote = !inQuote;

            }
            //when i find a comma use the position to 
            else if (line[i] == ',' && !inQuote)
            {
                //break everything in front of it into a word using substring
                //save it to the values array position
                values[valueIndex] = line.Substring(lastCommaIndex, (i - lastCommaIndex));
                //remember the position of the comma and set the start of the next string
                lastCommaIndex = i + 1;
                //increment value index
                valueIndex++;
            }
        }
        //get the last value
        values[valueIndex] = line.Substring(lastCommaIndex);

        return values;
    }


    void ParseCSV(string fileName)
    {
        //read the file lines into a string array
        records = System.IO.File.ReadAllLines(System.IO.Path.Combine(destPath, fileName));
        //count the comma's in the first line to know how many fields there are
        int fieldNum = 1; //number of fields = commas + 1
        string line = records[0]; //renames the record array element nicer to improve readability
        for (int i = 0; i < line.Length; i++) //checks every char in first line of data
        {
            //check if it is a comma and add to field num
            if (line[i] == ',')
            {
                fieldNum++;
            }
        }
        //create an array to hold the values of the line
        values = new string[fieldNum];
        //get the first line of the file to use as field names
        values = BreakIntoValues(line);
        //create the db file & create the table in the DB
        CreateTable(fileName);
        //loop through the file dealing with one record at a time (one line in CSV)
        for (int i = 1; i < records.Length; i++)
        {
            //loop through characters in the line looking for comma's 
            values = BreakIntoValues(records[i]);

            // add the values to the table as a new record by setting up a new query			
            string sql = "INSERT INTO menu (Name,Range,Description,KJ,Price,Image) " +
                         "VALUES('" + values[0] + "','" + values[1] + "','" + values[2] 
                         + "'," + values[3] + "," + values[4] + ",'" + values[5] + "');";

            //run the query
            RunMyQuery(sql);
        }//end loop
    }

    void GetCSVFile(string fileName)
    {
        //copy the file using a coroutine callback stream with the WWW class

        //set the source of the file
        //source = "http://eqsoc2184012/demer7/Pizza/" + fileName;
        source = "http://ihelensvaleshs.com/digitalsolutions/Pizza/" + fileName;

        //set the destination file path
        destFile = System.IO.Path.Combine(destPath, fileName);

        //start the coroutine to copy the file
        //runs a multi-threaded method
        //lets your "Yield" / wait until somethin happens before calling the thread
        StartCoroutine(CopyCSVCoroutine(fileName));
    }


    //Ienumertor return type is required to allow the necessary YIELD functionality / WAIT
    IEnumerator CopyCSVCoroutine(string fileName)
    {
        //start a connection to the file
        WWW www = new WWW(source);
        //wait for the connection to finish getting the file
        yield return www;

        //check for things that can and probably will go wrong EG is the data OK?
        if (www != null && www.isDone && www.error == null)
        {
            //start a file stream to pipe the binary data into a file
            FileStream stream = new FileStream(destFile, FileMode.Create);

            //write the data to the file
            stream.Write(www.bytes, 0, www.bytes.Length);
            //close the file stream
            stream.Close();

            //parse the file
            ParseCSV(fileName);
        }
    }


    // Use this for initialization
    void Start()
    {
        //set the save location for data on the device
        //persitent path can only be set in Start / Awake methods
        //we won't use streaming assets for most things as it can be read-only at runtime
        destPath = Application.persistentDataPath;

        //Get the CSV File "Menu.csv"
        GetCSVFile("Menu.csv");
    }

}
