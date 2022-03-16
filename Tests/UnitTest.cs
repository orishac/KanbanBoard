using System;
using IntroSE.Kanban.Backend.BusinessLayer;
using NUnit.Framework;
using Moq;
using IntroSE.Kanban.Backend.DataAccessLayer;
using System.Collections.Generic;


namespace Tests
{

    public class UnitTest
    {
        public IntroSE.Kanban.Backend.BusinessLayer.UserController userController;
        public Mock<User> user;
        public Mock<User> user2;
        public Dictionary<string, User> users;

        [SetUp]
        public void Setup()
        {
            user = new Mock<User>(new User("test@test.com", "Aa123", "test", "test@test.com"));

            users = new Dictionary<string, User>();

            user.Setup(k => k.login("Aa123")).Returns(user.Object);
            user.Setup(m => m.logout(false));

            user2 = new Mock<User>(new User("test2@test.com", "Aa123", "test2", "test2@test.com"));

            userController = new IntroSE.Kanban.Backend.BusinessLayer.UserController(users);


        }

        [Test]
        public void UserController_Register_ValidInput()
        {
            userController.DemiRegister("test@test.com", "Aa123", "test", "test@test.com");
            userController.login("test@test.com", "Aa123");
            Assert.AreEqual(userController.getUser("test@test.com").getEmail(), "test@test.com");
        }

        //invalid email input
        [TestCase ("test.com", "Aa123", "test2","test.com")]
        [TestCase("test@com", "Aa123", "test2", "test@com")]
        [TestCase("test1@test.com", "Aa123", "test2", "test1@com")]
        [TestCase ("@tests.com","Aa123", "test2", "test1@gmail.com")]

        //invlid nickname input
        [TestCase ("test1@gmail.com", "Aa123", "", "test1@gmail.com")]
        [TestCase("test1@gmail.com", "Aa123", "     ", "test1@gmail.com")]
        [TestCase("test1@gmail.com", "Aa123",null, "test1@gmail.com")]
        public void UserController_Register_Invalid_Email__Nickname(string email, string password, string nickname, string belongingBoard)
        {
            Exception thrown = Assert.Throws<Exception>(()=> userController.DemiRegister(email, password, nickname, belongingBoard));
            Assert.That(thrown.Message, Is.EqualTo("email or nickname or password are illegal"));

        }

   
        [TestCase("test1@gmail.com", "A123456", "test2", "test1@gmail.com")]
        [TestCase("test1@gmail.com", "a123456", "test2", "test1@gmail.com")]
        [TestCase("test1@gmail.com", "1", "test2", "test1@gmail.com")]
        [TestCase("test1@gmail.com", "Aa12", "test2", "test1@gmail.com")]
        [TestCase("test1@gmail.com", "#######", "test2", "test1@gmail.com")]
        [TestCase("test1@gmail.com", null, "test2", "test1@gmail.com")]
        [TestCase("test1@gmail.com", "    ", "test2", "test1@gmail.com")]
        [TestCase("test1@gmail.com", "", "test2", "test1@gmail.com")]
        public void UserController_Register_Invalid_Password(string email, string password, string nickname, string belongingBoard)
        {
            Exception thrown = Assert.Throws<Exception>(() => userController.DemiRegister(email, password, nickname, belongingBoard));
            Assert.That(thrown.Message, Is.EqualTo("password is illegal"));

        }

            
        [TestCase ("test@test.com", "Aa123", "test", "test@test.com")]
        public void UserController_Register_Exiting_User_Test(string email, string password, string nickname, string belongingBoard)
        {
            users.Add("test@test.com", user.Object);
            Exception thrown = Assert.Throws<Exception>(() => userController.DemiRegister(email, password, nickname, belongingBoard));
            Assert.That(thrown.Message, Is.EqualTo("there is an exsiting user with test@test.com"));
        }


        [Test]
        public void UserController_Login_Valid_Input()
        {
            users.Add("test@test.com", user.Object);
            userController.login("test@test.com", "Aa123");
            Assert.AreEqual(userController.getCurrentUser(), "test@test.com", "Test failed");

        }

        [Test]
        public void UserController_Login_Another_User_Logged_In()
        {
            users.Add("test@test.com", user.Object);
            users.Add("test2@test.com", user2.Object);
            userController.SetCurrentUser(user2.Object);
            Exception thrown = Assert.Throws<Exception>(() => userController.login("test@test.com", "Aa123"));
            Assert.That(thrown.Message, Is.EqualTo("only one user can be logged in"));

        }

        [Test]
        public void UserController_Login_User_Not_Exist()
        {
            Exception thrown = Assert.Throws<Exception>(() => userController.login("test@test.com", "Aa123"));
            Assert.That(thrown.Message, Is.EqualTo("the User is not registered"));

        }


        [Test]
        public void UserController_Logout_Valid_Input()
        {
            users.Add("test@test.com", user.Object);
            userController.SetCurrentUser(user.Object);
            userController.logout("test@test.com");
            Assert.AreEqual(userController.getCurrentUser(), null, "Test failed");
        }

        [Test]
        public void UserController_Logout_User_Not_Exist()
        {
            Exception thrown = Assert.Throws<Exception>(() => userController.logout("test@test.com"));
            Assert.That(thrown.Message, Is.EqualTo("user is not registered"));

        }

        [Test]
        public void UserController_Logout_User_Not_Logged_In()
        {
            users.Add("test@test.com", user.Object);
            Exception thrown = Assert.Throws<Exception>(() => userController.logout("test@test.com"));
            Assert.That(thrown.Message, Is.EqualTo("user is not logged in"));

        }












    }
}
