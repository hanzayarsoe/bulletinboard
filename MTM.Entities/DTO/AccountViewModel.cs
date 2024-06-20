namespace MTM.Entities.DTO
{
	public class AccountViewModel
	{
		#region Properties
		public string Id { get; set; }
		public string UserName { get; set; }
		public string FullName { get; set; }
		public int Role { get; set; }
		public string Email { get; set; }
		public bool IsActive {  get; set; }
		#endregion

		public AccountViewModel()
		{
			this.Id = string.Empty;
			this.UserName = string.Empty;
			this.FullName = string.Empty;
			this.Role = 0;
			this.Email = string.Empty;
			this.IsActive = false;
		}
	}
}
