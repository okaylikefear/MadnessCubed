using System;
using System.IO;
using UnityEngine;

namespace kube.map
{
	public class CubeGrid
	{
		public CubeGrid(int x, int y, int z)
		{
			this.sizeX = Mathf.CeilToInt((float)x / 16f);
			this.sizeY = Mathf.CeilToInt((float)y / 16f);
			this.sizeZ = Mathf.CeilToInt((float)z / 16f);
			this.chunks = new Chunk[this.sizeX, this.sizeY, this.sizeZ];
		}

		public void load(MemoryStream ms)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < this.sizeZ; i++)
			{
				for (int j = 0; j < this.sizeX; j++)
				{
					for (int k = 0; k < this.sizeY; k++)
					{
						int num3 = ms.ReadByte();
						if (num3 != 0)
						{
							Chunk chunk = this.chunks[j, k, i];
							chunk = new Chunk();
							chunk.y = ms.ReadByte();
							this.chunks[j, k, i] = chunk;
							if ((num3 & 1) == 1)
							{
								chunk.type = new byte[4096];
								ms.Read(chunk.type, 0, chunk.type.Length);
							}
							if ((num3 & 4) == 4)
							{
								chunk.xtype = new byte[2048];
								ms.Read(chunk.xtype, 0, chunk.xtype.Length);
							}
							if ((num3 & 2) == 2)
							{
								chunk.data = new byte[4096];
								ms.Read(chunk.data, 0, chunk.data.Length);
							}
						}
					}
				}
			}
			UnityEngine.Debug.Log("empty " + num);
			UnityEngine.Debug.Log("empty data" + num2);
		}

		public void save(MemoryStream ms)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < this.sizeZ; i++)
			{
				for (int j = 0; j < this.sizeX; j++)
				{
					for (int k = 0; k < this.sizeY; k++)
					{
						Chunk chunk = this.chunks[j, k, i];
						if (chunk == null)
						{
							ms.WriteByte(0);
							num++;
						}
						else
						{
							byte b = 0;
							if (chunk.type != null)
							{
								b = 1;
							}
							if (chunk.data != null)
							{
								b |= 2;
							}
							if (chunk.xtype != null)
							{
								b |= 4;
							}
							ms.WriteByte(b);
							ms.WriteByte((byte)chunk.y);
							if (chunk.type != null)
							{
								ms.Write(chunk.type, 0, chunk.type.Length);
							}
							if (chunk.xtype != null)
							{
								ms.Write(chunk.xtype, 0, chunk.xtype.Length);
							}
							if (chunk.data != null)
							{
								ms.Write(chunk.data, 0, chunk.data.Length);
							}
							else
							{
								num2++;
							}
						}
					}
				}
			}
			UnityEngine.Debug.Log("empty " + num);
			UnityEngine.Debug.Log("empty data" + num2);
		}

		public void set(int x, int y, int z, int type)
		{
			int num = x >> 4;
			int num2 = y >> 4;
			int num3 = z >> 4;
			int num4 = x & 15;
			int num5 = y & 15;
			int num6 = z & 15;
			Chunk chunk = this.chunks[num, num2, num3];
			if (chunk == null)
			{
				if (type == 0)
				{
					return;
				}
				chunk = new Chunk();
				chunk.y = num2;
				this.chunks[num, num2, num3] = chunk;
			}
			if (type != 0 && chunk.type == null)
			{
				chunk.type = new byte[4096];
			}
			if (type > 255 && chunk.xtype == null)
			{
				chunk.xtype = new byte[2048];
			}
			int num7 = num4 | num6 << 4 | num5 << 8;
			if (chunk.type != null)
			{
				chunk.type[num7] = (byte)type;
			}
			if (chunk.xtype != null)
			{
				int num8 = num7 & 1;
				int num9 = num7 >> 1;
				if (num8 == 0)
				{
					chunk.xtype[num9] = (byte)((int)(chunk.xtype[num9] & 240) | (type >> 8 & 15));
				}
				else
				{
					chunk.xtype[num9] = (byte)((int)(chunk.xtype[num9] & 15) | (type >> 4 & 240));
				}
			}
		}

		public void set(int x, int y, int z, int type, int data)
		{
			int num = x >> 4;
			int num2 = y >> 4;
			int num3 = z >> 4;
			int num4 = x & 15;
			int num5 = y & 15;
			int num6 = z & 15;
			Chunk chunk = this.chunks[num, num2, num3];
			if (chunk == null)
			{
				if (type == 0 && data == 0)
				{
					return;
				}
				chunk = new Chunk();
				chunk.y = num2;
				this.chunks[num, num2, num3] = chunk;
			}
			if (type != 0 && chunk.type == null)
			{
				chunk.type = new byte[4096];
			}
			if (type > 255 && chunk.xtype == null)
			{
				chunk.xtype = new byte[2048];
			}
			if (data != 0 && chunk.data == null)
			{
				chunk.data = new byte[4096];
			}
			int num7 = num4 | num6 << 4 | num5 << 8;
			if (chunk.type != null)
			{
				chunk.type[num7] = (byte)type;
			}
			if (chunk.xtype != null)
			{
				int num8 = num7 & 1;
				int num9 = num7 >> 1;
				if (num8 == 0)
				{
					chunk.xtype[num9] = (byte)((int)(chunk.xtype[num9] & 240) | (type >> 8 & 15));
				}
				else
				{
					chunk.xtype[num9] = (byte)((int)(chunk.xtype[num9] & 15) | (type >> 4 & 240));
				}
			}
			if (chunk.data != null)
			{
				chunk.data[num7] = (byte)data;
			}
		}

		public void setdata(int x, int y, int z, int data)
		{
			int num = x >> 4;
			int num2 = y >> 4;
			int num3 = z >> 4;
			int num4 = x & 15;
			int num5 = y & 15;
			int num6 = z & 15;
			Chunk chunk = this.chunks[num, num2, num3];
			if (chunk == null)
			{
				if (data == 0)
				{
					return;
				}
				chunk = new Chunk();
				chunk.y = num2;
				this.chunks[num, num2, num3] = chunk;
			}
			if (data != 0 && chunk.data == null)
			{
				chunk.data = new byte[4096];
			}
			int num7 = num4 | num6 << 4 | num5 << 8;
			if (chunk.data != null)
			{
				chunk.data[num7] = (byte)data;
			}
		}

		public int get(int x, int y, int z)
		{
			int num = x >> 4;
			int num2 = y >> 4;
			int num3 = z >> 4;
			int num4 = x & 15;
			int num5 = y & 15;
			int num6 = z & 15;
			Chunk chunk = this.chunks[num, num2, num3];
			if (chunk == null)
			{
				return 0;
			}
			if (chunk.type == null)
			{
				return 0;
			}
			int num7 = num4 | num6 << 4 | num5 << 8;
			if (chunk.xtype == null)
			{
				return (int)chunk.type[num7];
			}
			int num8 = num7 & 1;
			int num9 = num7 >> 1;
			if (num8 == 0)
			{
				return (int)(chunk.xtype[num9] & 15) << 8 | (int)chunk.type[num7];
			}
			return (int)(chunk.xtype[num9] & 240) << 4 | (int)chunk.type[num7];
		}

		public byte getdata(int x, int y, int z)
		{
			int num = x >> 4;
			int num2 = y >> 4;
			int num3 = z >> 4;
			int num4 = x & 15;
			int num5 = y & 15;
			int num6 = z & 15;
			Chunk chunk = this.chunks[num, num2, num3];
			if (chunk == null)
			{
				return 0;
			}
			if (chunk.data == null)
			{
				return 0;
			}
			int num7 = num4 | num6 << 4 | num5 << 8;
			return chunk.data[num7];
		}

		public int get(int x, int y, int z, ref int type, ref int data)
		{
			int num = x >> 4;
			int num2 = y >> 4;
			int num3 = z >> 4;
			int num4 = x & 15;
			int num5 = y & 15;
			int num6 = z & 15;
			Chunk chunk = this.chunks[num, num2, num3];
			if (chunk == null)
			{
				type = 0;
				data = 0;
				return 0;
			}
			if (chunk.type == null)
			{
				type = 0;
			}
			int num7 = num4 | num6 << 4 | num5 << 8;
			if (chunk.xtype != null)
			{
				int num8 = num7 & 1;
				int num9 = num7 >> 1;
				if (num8 == 0)
				{
					type = ((int)(chunk.xtype[num9] & 15) << 8 | (int)chunk.type[num7]);
				}
				else
				{
					type = ((int)(chunk.xtype[num9] & 240) << 4 | (int)chunk.type[num7]);
				}
			}
			type = (int)chunk.type[num7];
			if (chunk.data == null)
			{
				data = 0;
			}
			else
			{
				data = (int)chunk.data[num7];
			}
			return type;
		}

		public Chunk[,,] chunks;

		private int sizeX;

		private int sizeY;

		private int sizeZ;
	}
}
