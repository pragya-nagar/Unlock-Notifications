using OKRNotification.ViewModel.Request;
using OKRNotification.ViewModel.Response;
using System;
using System.Reflection;
using Xunit;

namespace OKRNotification.UnitTest.ViewModel
{
    public class ViewModelUnitTest
    {
        [Fact]
        public void MailLogRequestModel()
        {
            MailLogRequest model = new MailLogRequest();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }

        [Fact]

        public void MailRequestModel()
        {
            MailRequest model = new MailRequest();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }

        [Fact]

        public void NotificationDetailRequestModel()
        {
            NotificationDetailsRequest model = new NotificationDetailsRequest();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }


        [Fact]

        public void NotificationRequestModel()
        {
            NotificationsRequest model = new NotificationsRequest();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }

        [Fact]

        public void EmployeeResultModel()
        {
            EmployeeResult model = new EmployeeResult();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }

        [Fact]
        public void LoginUserDetailsModel()
        {
            LoginUserDetails model = new LoginUserDetails();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }
        [Fact]
        public void MailerTemplateResponseModel()
        {
            MailerTemplateResponse model = new MailerTemplateResponse();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }
        [Fact]
        public void NotificationResponseModel()
        {
            NotificationResponse model = new NotificationResponse();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }
        [Fact]
        public void NotificationsMessageTypeModel()
        {
            NotificationsMessageType model = new NotificationsMessageType();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }
        [Fact]
        public void RoleDetailsModel()
        {
            RoleDetails model = new RoleDetails();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }
        [Fact]
        public void UserIdentityModel()
        {
            UserIdentity model = new UserIdentity();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }
        [Fact]
        public void UserManagementResponseModel()
        {
            UserManagementResponse model = new UserManagementResponse();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }
        [Fact]
        public void UserResponseModel()
        {
            UserResponse model = new UserResponse();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }

        private T GetModelTestData<T>(T newModel)
        {
            Type type = newModel.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var propTypeInfo = type.GetProperty(prop.Name.Trim());
                if (propTypeInfo.CanRead)
                    prop.GetValue(newModel);
            }
            return newModel;
        }

        private T SetModelTestData<T>(T newModel)
        {
            Type type = newModel.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var propTypeInfo = type.GetProperty(prop.Name.Trim());
                var propType = prop.GetType();

                if (propTypeInfo.CanWrite)
                {
                    if (prop.PropertyType.Name == "String")
                    {
                        prop.SetValue(newModel, String.Empty);
                    }
                    else if (propType.IsValueType)
                    {
                        prop.SetValue(newModel, Activator.CreateInstance(propType));
                    }
                    else
                    {
                        prop.SetValue(newModel, null);
                    }
                }
            }
            return newModel;
        }

    }
}
