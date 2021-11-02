namespace OKRNotification.ViewModel.Response
{
    public class UserManagementResponse
    {
        public long userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool IsReset { get; set; }
        public string ImagePath { get; set; }
        public string ImageDetails { get; set; }
        public long roleId { get; set; }
        public string roleName { get; set; }
        public long employeeId { get; set; }
        public string emailId { get; set; }
        public int status { get; set; }
        public long reportingTo { get; set; }
        public string reportingName { get; set; }
    }
}
