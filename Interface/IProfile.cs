namespace StilSepetiApp.Interface
{
    public interface IProfile
    {
        int userId { get; set; }              
        string DisplayName { get; set; }     
        string Email { get; set; }          
        string Role { get; set; }             
        string? ProfileImageUrl { get; set; }
    }
}
