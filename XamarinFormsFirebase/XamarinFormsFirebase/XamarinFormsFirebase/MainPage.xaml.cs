using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Firebase.Xamarin.Database;
using System.Diagnostics;
using XamarinFormsFirebase.Classes;
using Firebase.Xamarin.Database.Query;
using Firebase.Xamarin.Auth;

namespace XamarinFormsFirebase
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

        }


        public async void OnceAsync()
        {
            var firebase = new FirebaseClient("https://yourdatabase.firebaseio.com/");
            var items = await firebase.Child("yourEntity").OnceAsync<MyClass>();

            foreach (var item in items)
            {
                Debug.WriteLine($"{item.Key} name is {item.Object.Name}");
            }
        }

        public async void SalvarAsync()
        {
            var firebase = new FirebaseClient("https://yourdatabase.firebaseio.com/");

            // add new item to list of data 

            var item = await firebase.Child("yourentity").PostAsync(new MyClass());

            // note that there is another overload for the PostAsync method which delegates the new key generation to the client

            Debug.WriteLine($"Key for the new item: {item.Key}");
          //  item = await firebase.Child("yourentity").Child("Ricardo").PutAsync(new MyClass());
            // add new item directly to the specified location (this will overwrite whatever data already exists at that location)

            //.WithAuth("<Authentication Token>") // <-- Add Auth token if required. Auth instructions further down in readme.

        }

        public  void Realtime()
        {
            var firebase = new FirebaseClient("https://dinosaur-facts.firebaseio.com/");
            var observable = firebase.Child("dinosaurs").AsObservable<MyClass>().Subscribe(d => Debug.WriteLine(d.Key));
        }

        public async void Autentication()
        {
            // Email/Password Auth
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("<google.firebase.com API Key>"));

            var auth = await authProvider.CreateUserWithEmailAndPasswordAsync("email@email.com", "password");

            // The auth Object will contain auth.User and the Authentication Token from the request
            System.Diagnostics.Debug.WriteLine(auth.FirebaseToken);


            // Facebook Auth
             authProvider = new FirebaseAuthProvider(new FirebaseConfig("<google.firebase.com API Key>"));
            var facebookAccessToken = "<login with facebook and get oauth access token>";

             auth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook, facebookAccessToken);

            // Using the Auth token to make requests.. (see more on requests below)
            var firebase = new FirebaseClient("https://dinosaur-facts.firebaseio.com/");
            var dinos = await firebase
              .Child("dinosaurs")
              .WithAuth(auth.FirebaseToken) // <-- Note the use of the Firebase Auth Token
              .OnceAsync<MyClass>();

            foreach (var dino in dinos)
            {
                System.Diagnostics.Debug.WriteLine($"{dino.Key} is {dino.Object}m high.");
            }
        }
    }
}
