using System;

internal class PunEvent
{
	public const byte MonoMessage = 199;

	public const byte RPC = 200;

	public const byte SendSerialize = 201;

	public const byte Instantiation = 202;

	public const byte CloseConnection = 203;

	public const byte Destroy = 204;

	public const byte RemoveCachedRPCs = 205;

	public const byte SendSerializeReliable = 206;

	public const byte DestroyPlayer = 207;

	public const byte AssignMaster = 208;
}
