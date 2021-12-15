namespace LibSharpRetro; 

public class UserOptionAttribute : Attribute {
	public readonly string Title, Category, HoverText, HelpUri;

	public UserOptionAttribute(string title, string category = "", string hoverText = null, string helpUri = null) {
		Title = title;
		Category = category;
		HoverText = hoverText;
		HelpUri = helpUri;
	}
}