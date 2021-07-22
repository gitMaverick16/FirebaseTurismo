using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase.Unity.Editor;
using Firebase;
using Firebase.Auth;
using TMPro;
using Firebase.Database;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System;
using Random = System.Random;
//using System.Drawing.Imaging;
using System.Drawing;
using Firebase.Extensions;
using Firebase.Storage;






public class AuthManager : MonoBehaviour
{


    StorageReference storageReference;
    FirebaseStorage storage;


    //Variables para captura
    public string folder = "ScreenshotFolder";

    //Variables de partida
    public int id_partida;

    //Variables para el control del tiempo
    Stopwatch tiempoNivelUno = new Stopwatch();
    Stopwatch tiempoNivelDos = new Stopwatch();
    DatabaseReference DBreference;
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;




    [Header("Game")]
    public TMP_InputField entrada;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField userRolField;
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;


    private void Start()
    {
        //InitializeFirebase();
        System.IO.Directory.CreateDirectory(folder);



    }
    private void Awake()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://fir-proyectoturismo.appspot.com");

        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    public void ClearRegisterFields()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));

    }

    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        ClearRegisterFields();
        ClearLoginFields();
    }

    //Function for the save button
    public void SaveData()
    {

        StartCoroutine(UpdateUsernameDatabase());

    }
    public void SaveLevelOne(double tiempo)
    {
        StartCoroutine(UpdateTimeLevelOne(tiempo));
    }
    public void SaveLevelTwo(double tiempo)
    {
        StartCoroutine(UpdateTimeLevelTwo(tiempo));
    }
    public void SaveAllLevel(double tiempo1, double tiempo2)
    {
        StartCoroutine(UpdateTotalTime(tiempo1, tiempo2));
    }


    public void SaveObjects()
    {
        StartCoroutine(UpdateObjects());
    }

    public void SaveDate()
    {
        StartCoroutine(UpdateDate());
    }
    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";

            yield return new WaitForSeconds(2);


            UIManager.instance.gameScreen();
            confirmLoginText.text = "";
            ClearLoginFields();
            ClearRegisterFields();
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        ClearLoginFields();
                        ClearRegisterFields();
                    }
                }
            }
        }
    }

    private IEnumerator UpdateUsernameAuth()
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = "pablito" };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase()
    {
        //Set the currently logged in user username in the database


        //
        var DBTask = DBreference.Child(User.UserId).Child("partida" + id_partida.ToString()).Child("ID de partida").SetValueAsync(id_partida.ToString());

        //
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateTimeLevelOne(double _tiempoNivelUno)
    {
        //Set the currently logged in user deaths
        var DBTask = DBreference.Child(User.UserId).Child("partida" + id_partida.ToString()).Child("Tiempo nivel uno").SetValueAsync(_tiempoNivelUno);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }
    private IEnumerator UpdateTimeLevelTwo(double _tiempoNivelDos)
    {
        //Set the currently logged in user deaths
        var DBTask = DBreference.Child(User.UserId).Child("partida" + id_partida.ToString()).Child("Tiempo nivel dos").SetValueAsync(_tiempoNivelDos);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }

    private IEnumerator UpdateTotalTime(double _tiempoNivelUno, double _tiempoNivelDos)
    {
        //Set the currently logged in user deaths
        var DBTask = DBreference.Child(User.UserId).Child("partida" + id_partida.ToString()).Child("tiempo nivel total").SetValueAsync((_tiempoNivelUno + _tiempoNivelDos));
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }

    private IEnumerator UpdateDate()
    {
        //Set the currently logged in user deaths
        DateTime fecha = DateTime.Now;
        fecha.ToString();
        var DBTask = DBreference.Child(User.UserId).Child("partida" + id_partida.ToString()).Child("Fecha").SetValueAsync((fecha));

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }

    private IEnumerator UpdateObjects()
    {
        //Set the currently logged in user deaths
        objetoRecogido objrec = new objetoRecogido("nombre del objeto", "descripcion del objeto");
        string json = JsonUtility.ToJson(objrec);
        var DBTask = DBreference.Child(User.UserId).Child("partida" + id_partida.ToString()).Child("objeto").SetRawJsonValueAsync(json);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }

    public IEnumerator esperar()
    {
        yield return new WaitForSeconds(2);
    }
    public void startLevelOne()
    {
        //esperar();
        id_partida = 0;
        id_partida = generarId();
        SaveData();
        //SaveDate();
        Console.WriteLine("Se incio el nivel 1");
        UIManager.instance.LevelOneScreen();
        tiempoNivelUno.Restart();
        tiempoNivelUno.Start();


    }

    public void startLevelTwo()
    {
        tiempoNivelUno.Stop();
        SaveLevelOne((tiempoNivelUno.Elapsed.TotalMilliseconds) / 1000);
        esperar();


        UIManager.instance.LevelTwoScreen();
        tiempoNivelDos.Restart();
        tiempoNivelDos.Start();


    }

    public void finishGame()
    {
        tiempoNivelDos.Stop();
        SaveLevelTwo((tiempoNivelDos.Elapsed.TotalMilliseconds) / 1000);
        SaveAllLevel((tiempoNivelUno.Elapsed.TotalMilliseconds) / 1000, (tiempoNivelDos.Elapsed.TotalMilliseconds) / 1000);
        esperar();


        UIManager.instance.gameScreen();
        tiempoNivelUno.Restart();
        tiempoNivelDos.Restart();




    }

    public int generarId()
    {
        Random aleatorio = new Random();
        int numero;
        numero = aleatorio.Next(1, 999999);
        return numero;
    }


    public class objetoRecogido
    {
        public string nombre;
        public string caracteristicas;

        public objetoRecogido()
        {

        }
        public objetoRecogido(String nombre, String caracteristicas)
        {
            this.nombre = nombre;
            this.caracteristicas = caracteristicas;
        }
    }



    public void takePhoto()
    {

        var texture = ScreenCapture.CaptureScreenshotAsTexture();


        var bytes = texture.EncodeToPNG();
        //Create a reference to where  the file needs to be uploated
        Random aleatorio = new Random();
        int numero;
        numero = aleatorio.Next(1, 999999);
        //Editing metadata
        var newMetaData = new MetadataChange();
        newMetaData.ContentType = "image/png";

        StorageReference uploadRef = storageReference.Child(User.UserId).Child(numero + " shot.png");
        Debug.Log("File upload started");
        uploadRef.PutBytesAsync(bytes, newMetaData).ContinueWithOnMainThread((task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
            }
            else
            {
                Debug.Log("File Uploaded Succesfully!");
            }
        });
    }
    private IEnumerator UploadCoroutine(Texture2D screenshot)
    {
        var storage = FirebaseStorage.DefaultInstance;
        var screenshotReference = storage.GetReference($"/screenshots/{Guid.NewGuid()}.png");
        var bytes = screenshot.EncodeToPNG();
        var uploadTask = screenshotReference.PutBytesAsync(bytes);
        yield return new WaitUntil(() => uploadTask.IsCompleted);

        if (uploadTask.Exception != null)
        {
            Debug.LogError($"Failed to upload because {uploadTask.Exception}");
            yield break;
        }
        var getUrlTask = screenshotReference.GetDownloadUrlAsync();
        yield return new WaitUntil(() => getUrlTask.IsCompleted);

        if (getUrlTask.Exception != null)
        {
            Debug.LogError($"Failed to get a dowload url with {getUrlTask.Exception}");
            yield break;
        }
        Debug.Log($"Download from {getUrlTask.Result}");
    }

}