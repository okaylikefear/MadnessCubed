using System;

public class AuthenticationValues
{
	public object AuthPostData { get; private set; }

	public virtual void SetAuthPostData(string stringData)
	{
		this.AuthPostData = ((!string.IsNullOrEmpty(stringData)) ? stringData : null);
	}

	public virtual void SetAuthPostData(byte[] byteData)
	{
		this.AuthPostData = byteData;
	}

	public virtual void SetAuthParameters(string user, string token)
	{
		this.AuthParameters = "username=" + Uri.EscapeDataString(user) + "&token=" + Uri.EscapeDataString(token);
	}

	public override string ToString()
	{
		return this.AuthParameters + " s: " + this.Secret;
	}

	public CustomAuthenticationType AuthType;

	public string AuthParameters;

	public string Secret;
}
