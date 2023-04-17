using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class SQLiteTasker
{
    static SQLiteTasker()
    {
        streamingAssetsPath = Application.streamingAssetsPath;
        persistentDataPath = Application.persistentDataPath;
    }
    
    private static Dictionary<string, Brick> brickDict;
    private static Dictionary<string, Color> colorDict;
    private static string persistentDataPath;
    private static string streamingAssetsPath;

    public static Dictionary<string, Brick> BrickDict
    {
        get
        {
            if (brickDict is null)
                return GetAllBrickDictAsync().Result;
            return brickDict;
        }
    }

    public async static void LoadFromSQLite()
    {
        IDbConnection dbConnection = await OpenDatabaseAsync();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        dbCommandReadValues.CommandText = "SELECT * FROM bricksCategories";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();
        while (dataReader.Read())
        {
            BrickDatabase.AddCategory(dataReader.GetString(0), Resources.Load<GameObject>(dataReader.GetString(1)), new List<BrickTag>());
        }

        foreach (var cat in BrickDatabase.BricksCategories)
        {
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "SELECT * FROM bricks WHERE catID = @id";
            IDbDataParameter idParameter = dbCommand.CreateParameter();
            idParameter.ParameterName = "@id";
            idParameter.Value = cat.Name;
            dbCommand.Parameters.Add(idParameter);

            IDataReader dbReader = dbCommand.ExecuteReader();
            while (dbReader.Read())
            {
                int argb = dbReader.GetInt32(2);
                var color = new Color(((argb >> 16) & 0xFF) / 255.0f,
                                        ((argb >> 8) & 0xFF) / 255.0f,
                                        (argb & 0xFF) / 255.0f,
                                        ((argb >> 24) & 0xFF) / 255.0f);
                cat.AddItem(dbReader.GetString(1), color);
            }
            dbReader.Close();
        }

        dataReader.Close();
        dbConnection.Close();
    }

    public static void UploadToSQLite()
    {
#if UNITY_EDITOR
        IDbConnection dbConnection = OpenDatabase();

        var cleaner = dbConnection.CreateCommand();
        cleaner.CommandText = "DELETE FROM bricksCategories; DELETE FROM bricks; VACUUM;";
        cleaner.ExecuteNonQuery();
        foreach (var cat in BrickDatabase.BricksCategories)
        {
            AddCategory(dbConnection, cat.Name, UnityEditor.AssetDatabase.GetAssetPath(cat.GM), null);
            foreach (var brick in cat.Bricks)
            {
                int argb = ((int)(brick.Color.a * 255.0f) << 24) |
                    ((int)(brick.Color.r * 255.0f) << 16) |
                    ((int)(brick.Color.g * 255.0f) << 8) |
                    (int)(brick.Color.b * 255.0f);
                AddBrick(dbConnection, cat.Name, brick.ID, argb);
            }
        }
#endif
    }


    public static async Task<Dictionary<string, Brick>> GetAllBrickDictAsync()
    {
        brickDict = new();
        colorDict = new();
        IDbConnection dbConnection = await OpenDatabaseAsync();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        dbCommandReadValues.CommandText = "SELECT * FROM bricks";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();
        while (dataReader.Read())
        {
            var cmd = dbConnection.CreateCommand();
            cmd.CommandText = "SELECT gameObjectPath FROM bricksCategories WHERE @brickName = bricksCategories.brickName";
            var dbDataParameter = cmd.CreateParameter();
            dbDataParameter.ParameterName = "@brickName";
            dbDataParameter.Value = dataReader.GetString(0);
            cmd.Parameters.Add(dbDataParameter);

            IDataReader dataReaderCat = cmd.ExecuteReader();
            dataReaderCat.Read();

            var brickFromRes = Resources.Load<Brick>(dataReaderCat.GetString(0));
            brickFromRes.ID = dataReader.GetString(1);

            int argb = dataReader.GetInt32(2);
            var color = new Color(((argb >> 16) & 0xFF) / 255.0f,
                                    ((argb >> 8) & 0xFF) / 255.0f,
                                    (argb & 0xFF) / 255.0f,
                                    ((argb >> 24) & 0xFF) / 255.0f);
            brickDict.Add(dataReader.GetString(1), brickFromRes);
            colorDict.Add(dataReader.GetString(1), color);
        }
        return brickDict;
    }

    public static Color GetColorById(string ID)
    {
        return colorDict[ID];
    }

    private static void AddCategory(IDbConnection dbConnection, string brickName, string path, int? tags)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "INSERT INTO bricksCategories (brickName, gameObjectPath, Tags) VALUES (@brickName, @gameObjectPath, @Tags)";

        IDbDataParameter value1Parameter = dbCommand.CreateParameter();
        value1Parameter.ParameterName = "@brickName";
        value1Parameter.Value = brickName;
        dbCommand.Parameters.Add(value1Parameter);

        IDbDataParameter value3Parameter = dbCommand.CreateParameter();
        value3Parameter.ParameterName = "@Tags";
        value3Parameter.Value = tags;
        dbCommand.Parameters.Add(value3Parameter);

        IDbDataParameter value2Parameter = dbCommand.CreateParameter();
        value2Parameter.ParameterName = "@gameObjectPath";
        if (path.Length > 1)
            value2Parameter.Value = path.Substring(17, path.Length - 24);
        else
            value2Parameter.Value = "";
        dbCommand.Parameters.Add(value2Parameter);

        dbCommand.ExecuteNonQuery();
    }

    private static void AddBrick(IDbConnection dbConnection, string catID, string brickName, int color)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "INSERT INTO bricks (catID, brick, color) VALUES (@catID, @brick, @color)";
        IDbDataParameter value1Parameter = dbCommand.CreateParameter();
        value1Parameter.ParameterName = "@catID";
        value1Parameter.Value = catID;
        dbCommand.Parameters.Add(value1Parameter);
        IDbDataParameter value2Parameter = dbCommand.CreateParameter();
        value2Parameter.ParameterName = "@brick";
        value2Parameter.Value = brickName;
        dbCommand.Parameters.Add(value2Parameter);
        IDbDataParameter value3Parameter = dbCommand.CreateParameter();
        value3Parameter.ParameterName = "@color";
        value3Parameter.Value = color;
        dbCommand.Parameters.Add(value3Parameter);
        dbCommand.ExecuteNonQuery();
    }

    private static IDbConnection OpenDatabase()
    {
        string dbUri = GetDatabasePath();
        IDbConnection dbConnection = new SqliteConnection("URI=file:" + dbUri);
        dbConnection.Open();
        return dbConnection;
    }


    private static string GetDatabasePath()
    {
#if UNITY_EDITOR
        return streamingAssetsPath + "/LegotekaDatabase.sqlite";
#endif

#if UNITY_ANDROID
#pragma warning disable CS0162
        string dbPath = persistentDataPath + "/LegotekaDatabase.sqlite";
        if (File.Exists(dbPath))
            File.Delete(dbPath);
        CopyDatabase();

        return dbPath;
#endif
    }


    private static void CopyDatabase()
    {
        string dbPath = streamingAssetsPath + "/LegotekaDatabase.sqlite";
        string copyPath = persistentDataPath + "/LegotekaDatabase.sqlite";

#pragma warning disable CS0618
        var www = new WWW(dbPath);

        while (!www.isDone) { }
        File.WriteAllBytes(copyPath, www.bytes);
    }









    #region async

    private static async Task CopyDatabaseAsync()
    {
        string dbPath = streamingAssetsPath + "/LegotekaDatabase.sqlite";
        string copyPath = persistentDataPath + "/LegotekaDatabase.sqlite";

        using (UnityWebRequest www = UnityWebRequest.Get(dbPath))
        {
            await SendWebRequestAsync(www);

            if (www.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(copyPath, www.downloadHandler.data);
            }
            else
            {
                Debug.LogError("Error downloading database: " + www.error);
            }
        }
    }


    private static Task SendWebRequestAsync(UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<bool>();

        var asyncOp = request.SendWebRequest();

        asyncOp.completed += (op) =>
        {
            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                tcs.SetException(new System.Exception(request.error));
            }
            else
            {
                tcs.SetResult(true);
            }
        };

        return tcs.Task;
    }



    private static async Task<IDbConnection> OpenDatabaseAsync()
    {
        string dbUri = GetDatabasePath();
        IDbConnection dbConnection = new SqliteConnection("URI=file:" + dbUri);
        await ((SqliteConnection)dbConnection).OpenAsync();
        return dbConnection;
    }




    #endregion

}
