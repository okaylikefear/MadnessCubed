using System;
using kube;
using UnityEngine;

public class MonsterScript : Pawn
{
	public bool isBoss
	{
		get
		{
			return this.healthMultiplier >= 3 || this.damageMultiplier >= 4;
		}
	}

	private void SetRespawnNum(int idRespawn)
	{
		this.createdFromRespawnNum = idRespawn;
	}

	private void SetHealthMultiplier(int _healthMultiplier)
	{
		this.healthMultiplier = _healthMultiplier;
		this.health *= (int)Mathf.Pow(2f, (float)this.healthMultiplier);
		this.maxHealth = this.health;
	}

	private void SetDamageMultiplier(int _damageMultiplier)
	{
		this.damageMultiplier = _damageMultiplier;
		this.hitDamage *= Mathf.Pow(2f, (float)this.damageMultiplier / 4f);
		this.shootDamage.x = this.shootDamage.x * Mathf.Pow(2f, (float)this.damageMultiplier / 4f);
		this.shootDamage.y = this.shootDamage.y * Mathf.Pow(2f, (float)this.damageMultiplier / 4f);
	}

	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		Kube.BCS = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControllerScript>();
		if (Kube.BCS == null)
		{
			Kube.BCS = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControllerScript>();
		}
		if (this.NO == null)
		{
			this.NO = Kube.BCS.NO;
		}
		this.initialized = true;
	}

	private void Start()
	{
		this.Init();
		this.id = -UnityEngine.Random.Range(1, 1000000000);
		this.PFMS = base.gameObject.GetComponent<PathFinderMoveScript>();
		if (this.weaponGO != null)
		{
			this.shootPointTransform = this.weaponGO.transform.Find("ShootPoint");
			this.weaponGOScript = this.weaponGO.GetComponent<WeaponScript>();
		}
		this.maxHealth = this.health;
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			this.type = 0;
			this.PFMS.SetPathFinderParams(this.runSpeed, this.jumpSpeed, this.height);
			if (Kube.BCS.gameType == GameType.mission && (Kube.BCS.missionType == ObjectsHolderScript.MissionType.reachTheExit || Kube.BCS.missionType == ObjectsHolderScript.MissionType.findNitems))
			{
				this.SetAngry(false);
			}
			else
			{
				this.SetAngry(true);
			}
		}
		else
		{
			this.type = 1;
			this.SendMyParams();
		}
		this.nextRoarTime = Time.time + UnityEngine.Random.Range(this.roarDeltaTime.x, this.roarDeltaTime.y);
		if (this.ragdoll != null)
		{
			this._ragDoll = (UnityEngine.Object.Instantiate(this.ragdoll, Vector3.zero, Quaternion.identity) as GameObject);
			this._ragDoll.SetActive(false);
		}
	}

	public void SendMyParams()
	{
		if (this.isDead)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_SendMyParams", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._SendMyParams(null);
		}
	}

	[RPC]
	private void _SendMyParams(PhotonMessageInfo info)
	{
		if (this.isDead)
		{
			return;
		}
		if (this.type == 0)
		{
			if (PhotonNetwork.room != null)
			{
				base.photonView.RPC("_HereAreMyParams", PhotonTargets.All, new object[]
				{
					this.createdFromRespawnNum,
					this.health,
					this.maxHealth,
					this.healthMultiplier,
					this.damageMultiplier,
					this.id
				});
			}
			else
			{
				this._HereAreMyParams(this.createdFromRespawnNum, this.health, this.maxHealth, this.healthMultiplier, this.damageMultiplier, this.id, null);
			}
		}
	}

	public void Startle()
	{
		if (this.isDead)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Startle", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._Startle(null);
		}
	}

	[RPC]
	private void _Startle(PhotonMessageInfo info)
	{
		this.Init();
		if (Kube.BCS.gameType == GameType.survival)
		{
			return;
		}
		if (this.isDead)
		{
			return;
		}
		if (this.type == 0)
		{
			int num = -1;
			float num2 = 1E+09f;
			GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
			this.targetsPlayers = null;
			this.targetsPlayers = new PlayerScript[array.Length];
			for (int i = 0; i < this.targetsPlayers.Length; i++)
			{
				this.targetsPlayers[i] = array[i].GetComponent<PlayerScript>();
			}
			this.lastSearchTargetListTime = Time.time;
			for (int j = 0; j < this.targetsPlayers.Length; j++)
			{
				if (!(this.targetsPlayers[j] == null))
				{
					if (!this.targetsPlayers[j].dead)
					{
						Vector3 vector = this.targetsPlayers[j].transform.position - base.transform.position;
						bool flag = true;
						if (!this.isAngry)
						{
							for (float num3 = 0f; num3 <= vector.magnitude; num3 += 0.5f)
							{
								Vector3 vector2 = base.transform.position + vector.normalized * num3;
								int num4 = Mathf.RoundToInt(vector2.x);
								int num5 = Mathf.RoundToInt(vector2.x);
								int num6 = Mathf.RoundToInt(vector2.x);
								if (num4 >= 0 && num5 >= 0 && num6 >= 0 && num4 < Kube.WHS.cubes.GetLength(0) && num5 < Kube.WHS.cubes.GetLength(1) && num6 < Kube.WHS.cubes.GetLength(2))
								{
									if (Kube.WHS.cubes[num4, num5, num6].phys == CubePhys.solid || Kube.WHS.cubes[num4, num5, num6].prop == CubeProps.closedDoor)
									{
										flag = false;
										break;
									}
								}
							}
						}
						if (vector.magnitude < num2 / 2f && ((vector.magnitude < this.angryDist && flag) || this.isAngry))
						{
							num = j;
							num2 = vector.magnitude;
						}
					}
				}
			}
			if (num != -1)
			{
				this.targetPlayer = this.targetsPlayers[num];
				this.targetType = 1;
				this.monsterBhv = 2;
			}
			if (this.animIdle.Length != 0)
			{
				base.animation.CrossFade(this.animIdle);
			}
			this.lastSearchTargetTime = Time.time;
		}
	}

	[RPC]
	private void _HereAreMyParams(int _createdFromRespawnNum, int _health, int _maxHealth, int _healthMultiplier, int _damageMultiplier, int _id, PhotonMessageInfo info)
	{
		if (this.isDead)
		{
			return;
		}
		if (this.type == 1)
		{
			this.createdFromRespawnNum = _createdFromRespawnNum;
			this.health = _health;
			this.maxHealth = _maxHealth;
			this.healthMultiplier = _healthMultiplier;
			this.damageMultiplier = _damageMultiplier;
			this.id = _id;
		}
	}

	private void SetAngry(bool _isAngry)
	{
		this.isAngry = _isAngry;
	}

	private void SetTargetPlayer(PlayerScript ps)
	{
		if (this.isDead)
		{
			return;
		}
		this.targetType = 1;
		this.targetPlayer = ps;
		this.monsterBhv = 2;
	}

	private void UpdateMonsterHit()
	{
		if (this.isDead)
		{
			return;
		}
		if (Time.time - this.lastSearchTargetListTime > this.searchTargetListDeltaTime)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
			this.targetsPlayers = null;
			this.targetsPlayers = new PlayerScript[array.Length];
			for (int i = 0; i < this.targetsPlayers.Length; i++)
			{
				this.targetsPlayers[i] = array[i].GetComponent<PlayerScript>();
			}
			this.lastSearchTargetListTime = Time.time;
		}
		if (Time.time - this.lastSearchTargetTime > this.searchTargetDeltaTime && this.targetsPlayers != null)
		{
			int num = -1;
			float num2 = 1E+09f;
			for (int j = 0; j < this.targetsPlayers.Length; j++)
			{
				if (!(this.targetsPlayers[j] == null))
				{
					if (!this.targetsPlayers[j].dead)
					{
						Vector3 from = this.targetsPlayers[j].transform.position - base.transform.position;
						bool flag = true;
						if (!this.isAngry)
						{
							for (float num3 = 0f; num3 <= from.magnitude; num3 += 0.5f)
							{
								Vector3 vector = base.transform.position + from.normalized * num3;
								int num4 = Mathf.RoundToInt(vector.x);
								int num5 = Mathf.RoundToInt(vector.x);
								int num6 = Mathf.RoundToInt(vector.x);
								if (num4 >= 0 && num5 >= 0 && num6 >= 0 && num4 < Kube.WHS.cubes.GetLength(0) && num5 < Kube.WHS.cubes.GetLength(1) && num6 < Kube.WHS.cubes.GetLength(2))
								{
									if (Kube.WHS.cubes[num4, num5, num6].phys == CubePhys.solid || Kube.WHS.cubes[num4, num5, num6].prop == CubeProps.closedDoor)
									{
										flag = false;
										break;
									}
								}
							}
						}
						if (from.magnitude < num2 && ((from.magnitude < this.angryDist && Vector3.Angle(from, base.transform.TransformDirection(Vector3.forward)) < this.viewAngle && flag) || this.isAngry))
						{
							num = j;
							num2 = from.magnitude;
						}
					}
				}
			}
			if (num != -1)
			{
				this.targetPlayer = this.targetsPlayers[num];
				this.targetType = 1;
				this.monsterBhv = 2;
			}
			if (this.animIdle.Length != 0)
			{
				base.animation.CrossFade(this.animIdle);
			}
			this.lastSearchTargetTime = Time.time;
		}
		if (this.monsterBhv == 2 && this.targetType == 1)
		{
			if (this.targetPlayer.dead || this.targetPlayer == null)
			{
				this.monsterBhv = 0;
				if (this.animIdle.Length != 0)
				{
					base.animation.CrossFade(this.animIdle);
				}
				return;
			}
			if ((this.targetPlayer.transform.position - base.transform.position).magnitude > this.distToHit)
			{
				this.PFMS.WalkingFollowTarget(this.targetPlayer.transform.position);
				base.animation.CrossFade(this.animRun);
				this.isShooting = false;
			}
			else
			{
				this.isShooting = true;
				if (this.animAttack.Length != 0)
				{
					base.animation.CrossFade(this.animAttack);
				}
				if (Time.time - this.lastHitTime > this.hitDeltaTime)
				{
					this.CreateHit();
					DamageMessage damageMessage = new DamageMessage();
					damageMessage.damage = (short)this.hitDamage;
					damageMessage.id_killer = this.id;
					damageMessage.team = 99;
					damageMessage.weaponType = -1;
					this.targetPlayer.SendMessage("ApplyDamage", damageMessage, SendMessageOptions.DontRequireReceiver);
					this.lastHitTime = Time.time;
				}
				this.moveDirection = new Vector3(0f, this.moveDirection.y, 0f);
			}
		}
	}

	private void UpdateMonsterShoot()
	{
		if (this.isDead)
		{
			return;
		}
		CubePhys cubePhysType = Kube.WHS.GetCubePhysType(base.transform.position + Vector3.up * 0.5f);
		if (Time.time - this.lastSearchTargetListTime > this.searchTargetListDeltaTime)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
			this.targetsPlayers = null;
			this.targetsPlayers = new PlayerScript[array.Length];
			for (int i = 0; i < this.targetsPlayers.Length; i++)
			{
				this.targetsPlayers[i] = array[i].GetComponent<PlayerScript>();
			}
			this.lastSearchTargetListTime = Time.time;
		}
		this.moveDirection.x = (this.moveDirection.z = 0f);
		if (cubePhysType == CubePhys.air)
		{
			if (!this.grounded)
			{
				this.moveDirection.y = this.moveDirection.y + Kube.OH.gravity * Time.deltaTime;
			}
			else
			{
				this.moveDirection.y = 0f;
			}
		}
		else if (cubePhysType == CubePhys.water)
		{
			if (!this.grounded)
			{
				this.moveDirection.y = Kube.OH.gravity * Time.deltaTime * 6f;
			}
			else
			{
				this.moveDirection.y = 0f;
			}
		}
		if (Time.time - this.lastSearchTargetTime > this.searchTargetDeltaTime && this.targetsPlayers != null)
		{
			int num = -1;
			float num2 = 1E+09f;
			for (int j = 0; j < this.targetsPlayers.Length; j++)
			{
				if (!(this.targetsPlayers[j] == null))
				{
					if (!this.targetsPlayers[j].dead)
					{
						Vector3 from = this.targetsPlayers[j].transform.position - base.transform.position;
						bool flag = true;
						if (!this.isAngry)
						{
							for (float num3 = 0f; num3 <= from.magnitude; num3 += 0.5f)
							{
								Vector3 vector = base.transform.position + from.normalized * num3;
								if (Kube.WHS.cubes[Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z)].phys == CubePhys.solid || Kube.WHS.cubes[Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z)].prop == CubeProps.closedDoor)
								{
									flag = false;
									break;
								}
							}
						}
						if (from.magnitude < num2 && ((from.magnitude < this.angryDist && Vector3.Angle(from, base.transform.TransformDirection(Vector3.forward)) < this.viewAngle && flag) || this.isAngry))
						{
							num = j;
							num2 = from.magnitude;
						}
					}
				}
			}
			if (num != -1)
			{
				this.targetPlayer = this.targetsPlayers[num];
				this.targetType = 1;
				this.monsterBhv = 2;
			}
			if (this.animIdle.Length != 0)
			{
				base.animation.CrossFade(this.animIdle);
			}
			this.lastSearchTargetTime = Time.time;
		}
		if (this.monsterBhv == 2 && this.targetType == 1)
		{
			if (this.targetPlayer.dead || this.targetPlayer == null)
			{
				this.monsterBhv = 0;
				if (this.animIdle.Length != 0)
				{
					base.animation.CrossFade(this.animIdle);
				}
				return;
			}
			Vector3 vector2 = this.targetPlayer.transform.position - base.transform.position;
			if (vector2.magnitude > this.minShootDist && !this.isShooting)
			{
				this.PFMS.WalkingFollowTarget(this.targetPlayer.transform.position);
				base.animation.CrossFade(this.animRun);
				if (vector2.magnitude < this.maxShootDist && Time.time > this.changeShootStateTime)
				{
					this.isShooting = true;
					this.changeShootStateTime = Time.time + UnityEngine.Random.Range(this.shootTime.x, this.shootTime.y);
				}
			}
			else if (!this.isShooting)
			{
				if (vector2.magnitude < this.maxShootDist && Time.time > this.changeShootStateTime)
				{
					this.isShooting = true;
					this.changeShootStateTime = Time.time + UnityEngine.Random.Range(this.shootTime.x, this.shootTime.y);
				}
			}
			else if (this.isShooting)
			{
				if (this.animAttack.Length != 0)
				{
					base.animation.CrossFade(this.animAttack);
				}
				if (Time.time - this.lastShootTime > this.shootDeltaTime)
				{
					CubePhys cubePhysType2 = Kube.WHS.GetCubePhysType(this.shootPointTransform.position);
					if (cubePhysType2 != CubePhys.solid)
					{
						this.CreateShot(this.targetPlayer.transform.position + Vector3.up);
					}
					this.lastShootTime = Time.time;
				}
				if (Time.time > this.changeShootStateTime)
				{
					this.isShooting = false;
					this.changeShootStateTime = Time.time + UnityEngine.Random.Range(this.noShootTime.x, this.noShootTime.y);
				}
			}
			if (this.moveDirection.x + this.moveDirection.z > 0.5f && this.animRun.Length != 0)
			{
				base.animation.CrossFade(this.animRun);
			}
			base.transform.LookAt(new Vector3(this.targetPlayer.transform.position.x, base.transform.position.y, this.targetPlayer.transform.position.z));
		}
	}

	private void UpdateMonsterShootHitMagic()
	{
		if (this.isDead)
		{
			return;
		}
		CubePhys cubePhysType = Kube.WHS.GetCubePhysType(base.transform.position + Vector3.up * 0.5f);
		if (Time.time - this.lastSearchTargetListTime > this.searchTargetListDeltaTime)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
			this.targetsPlayers = null;
			this.targetsPlayers = new PlayerScript[array.Length];
			for (int i = 0; i < this.targetsPlayers.Length; i++)
			{
				this.targetsPlayers[i] = array[i].GetComponent<PlayerScript>();
			}
			this.lastSearchTargetListTime = Time.time;
		}
		this.moveDirection.x = (this.moveDirection.z = 0f);
		if (cubePhysType == CubePhys.air)
		{
			if (!this.grounded)
			{
				this.moveDirection.y = this.moveDirection.y + Kube.OH.gravity * Time.deltaTime;
			}
			else
			{
				this.moveDirection.y = 0f;
			}
		}
		else if (cubePhysType == CubePhys.water)
		{
			if (!this.grounded)
			{
				this.moveDirection.y = Kube.OH.gravity * Time.deltaTime * 6f;
			}
			else
			{
				this.moveDirection.y = 0f;
			}
		}
		if (Time.time - this.lastSearchTargetTime > this.searchTargetDeltaTime && this.targetsPlayers != null)
		{
			int num = -1;
			float num2 = 1E+09f;
			for (int j = 0; j < this.targetsPlayers.Length; j++)
			{
				if (!(this.targetsPlayers[j] == null))
				{
					if (!this.targetsPlayers[j].dead)
					{
						Vector3 from = this.targetsPlayers[j].transform.position - base.transform.position;
						bool flag = true;
						if (!this.isAngry)
						{
							for (float num3 = 0f; num3 <= from.magnitude; num3 += 0.5f)
							{
								Vector3 vector = base.transform.position + from.normalized * num3;
								if (Kube.WHS.cubes[Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z)].phys == CubePhys.solid || Kube.WHS.cubes[Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z)].prop == CubeProps.closedDoor)
								{
									flag = false;
									break;
								}
							}
						}
						if (from.magnitude < num2 && ((from.magnitude < this.angryDist && Vector3.Angle(from, base.transform.TransformDirection(Vector3.forward)) < this.viewAngle && flag) || this.isAngry))
						{
							num = j;
							num2 = from.magnitude;
						}
					}
				}
			}
			if (num != -1)
			{
				this.targetPlayer = this.targetsPlayers[num];
				this.targetType = 1;
				this.monsterBhv = 2;
			}
			if (this.animIdle.Length != 0)
			{
				base.animation.CrossFade(this.animIdle);
			}
			this.lastSearchTargetTime = Time.time;
		}
		if (this.monsterBhv == 2 && this.targetType == 1)
		{
			if (this.targetPlayer.dead || this.targetPlayer == null)
			{
				this.monsterBhv = 0;
				if (this.animIdle.Length != 0)
				{
					base.animation.CrossFade(this.animIdle);
				}
				return;
			}
			Vector3 vector2 = this.targetPlayer.transform.position - base.transform.position;
			if (Time.time > this.nextMagicTime && !this.isMagic)
			{
				this.isMagic = true;
				this.timeMagicDone = Time.time + this.magicTimeToDo;
				this.nextMagicTime = Time.time + UnityEngine.Random.Range(this.magicNextTimeRandom.x, this.magicNextTimeRandom.y);
				UnityEngine.Object.Instantiate(this.magicGO, base.transform.position, base.transform.rotation);
				base.animation.Play(this.animMagic);
			}
			else if (this.isMagic && Time.time > this.timeMagicDone)
			{
				this.isMagic = false;
			}
			else if (!this.isMagic)
			{
				if (vector2.magnitude < this.distToHit)
				{
					this.isHitting = true;
					if (this.animAttack.Length != 0)
					{
						base.animation.CrossFade(this.animAttackHit);
					}
					if (Time.time - this.lastHitTime > this.hitDeltaTime)
					{
						this.CreateHit();
						DamageMessage damageMessage = new DamageMessage();
						damageMessage.damage = (short)this.hitDamage;
						damageMessage.id_killer = this.id;
						damageMessage.team = 99;
						damageMessage.weaponType = -1;
						this.targetPlayer.SendMessage("ApplyDamage", damageMessage, SendMessageOptions.DontRequireReceiver);
						this.lastHitTime = Time.time;
					}
					this.moveDirection = new Vector3(0f, this.moveDirection.y, 0f);
				}
				else if (!this.isShooting)
				{
					this.PFMS.WalkingFollowTarget(this.targetPlayer.transform.position);
					base.animation.CrossFade(this.animRun);
					if (vector2.magnitude < this.maxShootDist && Time.time > this.changeShootStateTime)
					{
						this.isShooting = true;
						this.changeShootStateTime = Time.time + UnityEngine.Random.Range(this.shootTime.x, this.shootTime.y);
					}
				}
				else if (this.isShooting)
				{
					if (this.animAttack.Length != 0)
					{
						base.animation.CrossFade(this.animAttack);
					}
					if (Time.time - this.lastShootTime > this.shootDeltaTime)
					{
						CubePhys cubePhysType2 = Kube.WHS.GetCubePhysType(this.shootPointTransform.position);
						if (cubePhysType2 != CubePhys.solid)
						{
							this.CreateShot(this.targetPlayer.transform.position + Vector3.up);
						}
						this.lastShootTime = Time.time;
					}
					if (Time.time > this.changeShootStateTime)
					{
						this.isShooting = false;
						this.changeShootStateTime = Time.time + UnityEngine.Random.Range(this.noShootTime.x, this.noShootTime.y);
					}
				}
			}
			if (this.moveDirection.x + this.moveDirection.z > 0.5f && this.animRun.Length != 0)
			{
				base.animation.CrossFade(this.animRun);
			}
			base.transform.LookAt(new Vector3(this.targetPlayer.transform.position.x, base.transform.position.y, this.targetPlayer.transform.position.z));
		}
	}

	private void CreateShot(Vector3 shotPoint)
	{
		if (this.isDead)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateShot", PhotonTargets.All, new object[]
			{
				shotPoint
			});
		}
		else
		{
			this._CreateShot(shotPoint, null);
		}
	}

	[RPC]
	private void _CreateShot(Vector3 shotPoint, PhotonMessageInfo info)
	{
		if (this.isDead)
		{
			return;
		}
		DamageMessage damageMessage = new DamageMessage();
		if (PhotonNetwork.offlineMode || base.photonView.isMine)
		{
			damageMessage.damage = (short)UnityEngine.Random.Range(this.shootDamage.x, this.shootDamage.y);
		}
		else
		{
			damageMessage.damage = 0;
		}
		damageMessage.id_killer = 0;
		damageMessage.team = 99;
		if (this.weaponGOScript != null)
		{
			this.weaponGOScript.WeaponShot(this.bulletPrefab, shotPoint, damageMessage);
		}
	}

	private void CreateHit()
	{
		if (this.isDead)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_CreateHit", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._CreateHit(null);
		}
	}

	[RPC]
	private void _CreateHit(PhotonMessageInfo info)
	{
		if (this.isDead)
		{
			return;
		}
		if (this.attackSound != null)
		{
			UnityEngine.Object.Instantiate(this.attackSound, base.transform.position, Quaternion.identity);
		}
	}

	private void Update()
	{
		if (this.isDead)
		{
			return;
		}
		if (this.freezed)
		{
			return;
		}
		if (Time.time > this.nextRoarTime)
		{
			if (this.roarSound != null)
			{
				UnityEngine.Object.Instantiate(this.roarSound, base.transform.position, Quaternion.identity);
			}
			this.nextRoarTime = Time.time + UnityEngine.Random.Range(this.roarDeltaTime.x, this.roarDeltaTime.y);
		}
		if (this.type == 0)
		{
			if (this.monsterType == MonsterScript.MonsterType.hit)
			{
				this.UpdateMonsterHit();
			}
			else if (this.monsterType == MonsterScript.MonsterType.shoot)
			{
				this.UpdateMonsterShoot();
			}
			else if (this.monsterType == MonsterScript.MonsterType.shootHitMagic)
			{
				this.UpdateMonsterShootHitMagic();
			}
		}
		else if (this.type == 1)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.correctPlayerPos, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.correctPlayerRot, Time.deltaTime * (float)PhotonNetwork.sendRateOnSerialize);
		}
	}

	private void ApplyDamage(DamageMessage dm)
	{
		if (this.isDead)
		{
			return;
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_ApplyDamage", PhotonTargets.All, new object[]
			{
				dm.damage,
				dm.id_killer,
				dm.team,
				dm.weaponType
			});
		}
		else
		{
			this._ApplyDamage(dm.damage, dm.id_killer, dm.team, dm.weaponType, null);
		}
	}

	[RPC]
	private void _ApplyDamage(short _damage, int _id_killer, int _team, short _weaponType, PhotonMessageInfo info)
	{
		if (this.isDead)
		{
			return;
		}
		if (this.type == 0)
		{
			this.health -= (int)_damage;
			if (this.health <= 0)
			{
				this.Die(_id_killer, this.pointsForKillMe);
			}
		}
	}

	private void Die(int id_killer, int myPoints)
	{
		if (this.isDead)
		{
			return;
		}
		if (this.createdFromRespawnNum != -1)
		{
			this.NO.MonsterDead(this.createdFromRespawnNum);
		}
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Die", PhotonTargets.All, new object[]
			{
				id_killer,
				myPoints
			});
		}
		else
		{
			this._Die(id_killer, myPoints, null);
		}
	}

	[RPC]
	private void _Die(int id_killer, int myPoints, PhotonMessageInfo info)
	{
		this.Init();
		if (this.isDead)
		{
			return;
		}
		this.isDead = true;
		this._ragDoll.SetActive(true);
		MonsterScript.CopyTransformsRecurse(base.transform, this._ragDoll.transform);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < array.Length; i++)
		{
			PlayerScript component = array[i].GetComponent<PlayerScript>();
			if (!(component == null))
			{
				if (component.id == id_killer && id_killer != this.id)
				{
					array[i].SendMessage("YouKilledMonster", myPoints);
					break;
				}
			}
		}
		if (Kube.GPS.playerId == id_killer)
		{
			(UnityEngine.Object.Instantiate(Kube.OH.pointsText, base.transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject).SendMessage("SetText", "+" + myPoints);
			BattleControllerScript bcs = Kube.BCS;
			bcs.bonusCounters.zombieKill = bcs.bonusCounters.zombieKill + 1;
			if (this.monsterNum == 4)
			{
				BattleControllerScript bcs2 = Kube.BCS;
				bcs2.bonusCounters.demonKilled = bcs2.bonusCounters.demonKilled + 1;
			}
		}
		if (this.deadSound != null)
		{
			UnityEngine.Object.Instantiate(this.deadSound, base.transform.position, Quaternion.identity);
		}
		if (Kube.BCS.gameType == GameType.survival)
		{
			Kube.BCS.MonsterDead();
		}
		if (this.type == 0)
		{
			base.Invoke("DestroyPhotonView", 2f);
		}
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
		}
		UnityEngine.Object.Destroy(base.gameObject.collider);
	}

	private void DestroyPhotonView()
	{
		PhotonNetwork.Destroy(base.gameObject);
	}

	private static void CopyTransformsRecurse(Transform src, Transform dst)
	{
		dst.position = src.position;
		dst.rotation = src.rotation;
		if (dst.gameObject.rigidbody != null)
		{
			dst.gameObject.rigidbody.Sleep();
		}
		foreach (object obj in dst)
		{
			Transform transform = (Transform)obj;
			Transform transform2 = src.Find(transform.name);
			if (transform2)
			{
				MonsterScript.CopyTransformsRecurse(transform2, transform);
			}
		}
	}

	private void OnGUI()
	{
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (PhotonNetwork.connected)
		{
			if (stream.isWriting)
			{
				if (Time.time - this.lastSendProps > 5f)
				{
					stream.SendNext(1);
					this.lastSendProps = Time.time;
				}
				else
				{
					stream.SendNext(2);
				}
				stream.SendNext(base.transform.position);
				stream.SendNext(base.transform.rotation);
				stream.SendNext(this.isShooting);
				stream.SendNext(this.isHitting);
			}
			else
			{
				byte b = (byte)stream.ReceiveNext();
				if (b == 1)
				{
				}
				this.correctPlayerPos = (Vector3)stream.ReceiveNext();
				this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
				bool flag = (bool)stream.ReceiveNext();
				bool flag2 = (bool)stream.ReceiveNext();
				if (!flag && !flag2)
				{
					base.animation.CrossFade(this.animRun);
				}
				else if (flag)
				{
					base.animation.CrossFade(this.animAttack);
				}
				else if (flag2)
				{
					base.animation.CrossFade(this.animAttackHit);
				}
			}
		}
	}

	private void Freeze(FreezeStruct fs)
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_Freeze", PhotonTargets.All, new object[]
			{
				fs.freezeTime
			});
		}
		else
		{
			this._Freeze(fs.freezeTime, null);
		}
	}

	[RPC]
	private void _Freeze(float freezeTime, PhotonMessageInfo info)
	{
		if (base.photonView.isMine)
		{
			base.Invoke("UnFreeze", freezeTime);
		}
		this.freezed = true;
	}

	private void UnFreeze()
	{
		if (PhotonNetwork.room != null)
		{
			base.photonView.RPC("_UnFreeze", PhotonTargets.All, new object[0]);
		}
		else
		{
			this._UnFreeze(null);
		}
	}

	[RPC]
	private void _UnFreeze(PhotonMessageInfo info)
	{
		this.freezed = false;
	}

	private int monsterBhv;

	public MonsterScript.MonsterType monsterType;

	public int monsterNum;

	public string animRun;

	public string animIdle;

	public string animAttack;

	public string animAttackHit;

	public float runSpeed = 5f;

	public float runSpeedBonus;

	public float jumpSpeed = 10f;

	public float jumpSpeedBonus;

	public int height = 2;

	public float viewAngle = 60f;

	public float angryDist = 50f;

	public float searchTargetDeltaTime = 0.5f;

	public float searchTargetListDeltaTime = 5f;

	public float distToHit = 1.5f;

	public float hitDeltaTime = 1f;

	private float lastHitTime;

	public float hitDamage = 10f;

	private float lastSearchTargetListTime;

	private float lastSearchTargetTime;

	private NetworkObjectScript NO;

	public int type;

	public bool paused;

	private PlayerScript[] targetsPlayers;

	private int targetType;

	private PlayerScript targetPlayer;

	private int id;

	public int health = 20;

	public int maxHealth;

	private int healthMultiplier;

	private int damageMultiplier;

	public int pointsForKillMe = 10;

	public GameObject ragdoll;

	private float nextRoarTime;

	public Vector2 roarDeltaTime;

	public GameObject roarSound;

	public GameObject attackSound;

	public GameObject deadSound;

	private PathFinderMoveScript PFMS;

	private Transform shootPointTransform;

	public int createdFromRespawnNum = -1;

	public Vector2 magicNextTimeRandom;

	public GameObject magicGO;

	private float nextMagicTime;

	private float timeMagicDone;

	public float magicTimeToDo;

	public string animMagic;

	private bool isMagic;

	private bool initialized;

	private bool isAngry;

	private bool grounded = true;

	private Vector3 moveDirection;

	private float lastShootTime;

	public float shootDeltaTime = 0.5f;

	private bool isShooting;

	private bool isHitting;

	public GameObject weaponGO;

	private float changeShootStateTime;

	public Vector2 shootTime;

	public Vector2 noShootTime;

	public float maxShootDist;

	public float minShootDist;

	public GameObject bulletPrefab;

	private WeaponScript weaponGOScript;

	public Vector2 shootDamage;

	protected bool isDead;

	private GameObject _ragDoll;

	private Vector3 correctPlayerPos = new Vector3(-10000f, -10000f, 0f);

	private Quaternion correctPlayerRot = Quaternion.identity;

	private float lastSendProps;

	private bool freezed;

	private enum MonsterBhv
	{
		idle,
		idleWalkAround,
		attack
	}

	public enum MonsterType
	{
		hit,
		shootHit,
		shoot,
		flyingBite,
		flyingShoot,
		shootHitMagic
	}

	private enum TargetType
	{
		none,
		player
	}
}
