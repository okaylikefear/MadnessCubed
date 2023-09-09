using System;
using System.Collections.Generic;

public class Localize
{
	public static Dictionary<string, string> hash
	{
		get
		{
			return Localize._hash;
		}
	}

	public static string T(string key)
	{
		if (Localize._hash.ContainsKey(key))
		{
			return Localize._hash[key];
		}
		if (Localize._tt.GetField(key) == null)
		{
			return key;
		}
		Localize._hash[key] = (string)Localize._tt.GetField(key).GetValue(null);
		return Localize._hash[key];
	}

	public static string init_nickname = "Введите имя персонажа:";

	public static string common_connect = "Подождите, идёт соединение с игровым сервером...";

	public static string common_cancel = "Отмена";

	public static string mission_gift = "Награда:";

	public static string mission_done = "Уровень пройден!";

	public static string mission_nobonus = "Золото и игровые предметы выдаются только при ПЕРВОМ прохождении миссии.";

	public static string game_fail = "Никто не выжил...";

	public static string dialog_play = "ИГРАТЬ!";

	public static string dialog_back = "Назад";

	public static string dialog_ok = "OK";

	public static string postonwall = "Рассказать";

	public static string episode1 = "Начало";

	public static string episode2 = "Мясорубка";

	public static string episode3 = "Мертвецы";

	public static string episode4 = "Выживший";

	public static string episode5 = "Заражение";

	public static string episode_name = "Эпизод {0}";

	public static string mm_missions = "Миссии";

	public static string mm_maps = "Мои карты";

	public static string mm_online = "Сетевая битва";

	public static string mm_shop = "Инвентарь (С)";

	public static string mm_bank = "Банк";

	public static string mm_options = "Опции игры(O)";

	public static string mm_invite = "Пригласите своих друзей!";

	public static string c_slot = "Карта";

	public static string c_new_slot = "Новый слот";

	public static string c_buy_slot_gold = "Для покупки слота не хватает монет:";

	public static string map_loading_data = "Загружаются данные карты...";

	public static string low_gold = "--- Не хватает монет. ---";

	public static string c_map_title = "Название карты:";

	public static string c_map_id = "ID карты:";

	public static string c_play_alone = "Не мешать!\n(На Вашу карту не будут заходить)";

	public static string c_light_type = "Тип освещения:\n";

	public static string c_map_choseMap = "Выберите карту из списка Ваших карт слева.";

	public static string c_map_help = "Вы можете строить свои карты в одиночку или с друзьями, а затем играть на своих картах по сети!";

	public static string common_connect_game = "Подождите, идёт подсоединение к игре...";

	public static string password = "Пароль";

	public static string do_load = "ЗАГРУЗИТЬ";

	public static string regenerate = "Перегенерировать";

	public static string ys_regenerate = "Текущая карта будет стёрта!\nТочно перегенегировать?";

	public static string no = "НЕТ";

	public static string yes = "Да";

	public static string ok = "OK";

	public static string map_slot_empty = "Карта ещё не создана.\nВыбирете тип новой карты:";

	public static string create_game = "СОЗДАТЬ";

	public static string dialog_cancel = "Отмена";

	public static string online_server_type = "ВЫБЕРИТЕ ТИП СЕРВЕРА:";

	public static string players_on_server = "На сервере с выбранными параметрами играет  {0} игроков";

	public static string connect_game = "ПРИСОЕДИНИТЬСЯ К СЕРВЕРУ!";

	public static string onl_type = "Тип: ";

	public static string onl_map = "Карта:";

	public static string onl_light = "Свет:";

	public static string onl_break = "Разрушения:";

	public static string onl_input_password = "Пароль для входа: ";

	public static string onl_friends = "ИГРЫ, ГДЕ СЕЙЧАС ИГРАЮТ МОИ ДРУЗЬЯ:";

	public static string onl_empty_rooms = "Игр с друзьями нет..";

	public static string onl_unknown_map = "Неизвестная карта";

	public static string onl_mission_desc = "Эпизод {0} , миссия {1} ";

	public static string onl_players_online = "Онлайн: {0}";

	public static string daily_bonus = "БОНУС";

	public static string daily_desc = "Заходи в игру каждый день и получишь уникальную деталь от секретного оружия";

	public static string daily_tip = "Собери 4 запчасти и получи \"{0}\"";

	public static string take_bonus = "Забрать бонус";

	public static string dont_cheat = "Я не буду больше читерить...";

	public static string special_bonus = "СПЕЦИАЛЬНЫЙ БОНУС!";

	public static string is_one_day = "1 день";

	public static string is_one_week = "7 дней";

	public static string is_one_mounth = "30 дней";

	public static string is_two_weeks = "2 недели";

	public static string is_unlimit = "Навсегда";

	public static string no_money = "Не хватает денег:";

	public static string is_reload = "R - перезарядка";

	public static string is_time_left = "Осталось ";

	public static string is_days_left = "{0} дней ";

	public static string is_hours_left = "{0} часов {1} минут";

	public static string is_max = "Максимальный запас";

	public static string is_min_level_to_buy = "Для увел. нужен";

	public static string is_ammo_info = "Этот набор патронов будет выдаваться каждый бой.";

	public static string is_initial_ammo = "Начальный запас:";

	public static string[] is_tabs = new string[]
	{
		"Кубы",
		"Декорации",
		"Механизмы",
		"Предметы и\nспособности",
		"Оружие",
		"Перс",
		"Банк"
	};

	public static string[] BonusTypeStr = new string[]
	{
		"здоровье",
		"броня",
		"скорость",
		"прыжок",
		"стойкость"
	};

	public static string vip_info = "Получите статус V.I.P. и у Вас будет:\n1. +{0}% опыта и денег за уровни!\n2. Специальные формы кубов!\n3. Все характеристики на 2 уровня выше!\n4. Возрождение за 10 сек. в режиме выживания!\n5. Возможность разговора по микрофону прямо в игре!\n";

	public static string is_weapon_power = "Мощность: ";

	public static string is_cash = "баксов:";

	public static string is_gold = "монет:";

	public static string is_upgrade = "Увеличить - ";

	public static string is_weapon_ammo_has = "Обойм в наличии: ";

	public static string is_weapon_ammo_buy = "\tКупить";

	public static string is_need_more = " - нужно ещё ";

	public static string is_buy_for = "Купить за ";

	public static string player_level = "Уровень: ";

	public static string player_rank = "Текущий ранг: ";

	public static string player_kills = "Фрагов:  ";

	public static string xp_next = "опыта до сл. уровня";

	public static string is_nick = "Имя:";

	public static string is_no_bonus = "Без бонусов";

	public static string is_no_item = "Ничего";

	public static string mission_developing = "Миссия в разработке";

	public static string mission_closed = "Пройдите предыдущую миссию на 3 звезды.";

	public static string episode_developing = "Эпизод в разработке";

	public static string[] MissionType = new string[]
	{
		"<empty>",
		"Найти выход",
		"Продержаться {0}",
		"Убить {0} {1} ",
		"Найти {0}",
		"Найти {0} за {1}",
		"Убить {0} {1} за {2}"
	};

	public static string[] CubesTypes = new string[]
	{
		"Природные",
		"Строительные",
		"Декоративные",
		"Стекло",
		"Вода",
		"Металлы"
	};

	public static string[] DecorTypes = new string[]
	{
		"Свет",
		"Мебель",
		"Двери",
		"Лестницы",
		"Растения",
		"Декор",
		"Респаун",
		"Дороги",
		"Оружия",
		"Монстры"
	};

	public static string[] ItemsTypes = new string[]
	{
		"Строительные",
		"Спец. предметы",
		"Боевые",
		"Движения"
	};

	public static string[] WeaponTypes = new string[]
	{
		"Оружие",
		"Патроны"
	};

	public static string[] CharacterPages = new string[]
	{
		"Сводка",
		"Одежда",
		"Параметры",
		"Статус V.I.P."
	};

	public static string[] DeviceTypes = new string[]
	{
		"Переключатели\nи провода",
		"Двери, лифты\nплатформы...",
		"Транспорт",
		"Другие механизмы"
	};

	public static string[] ClothesType = new string[]
	{
		"Скины",
		"Голова",
		"Торс",
		"Спина",
		"Руки",
		"Ноги",
		"Плечи"
	};

	public static string[] monsterName = new string[]
	{
		"Зомби",
		"Агент",
		"Зомби-солдат",
		"Зомби-шахтёр",
		"Демон",
		"Резак",
		"DEAD-Мороз"
	};

	public static string[] AAnames = new string[]
	{
		"Дверь вертикальная",
		"Дверь горизонтальная",
		"Лифт",
		"Силовое поле"
	};

	public static string[] triggerTypeName = new string[]
	{
		"Переключатель (вкл/выкл)",
		"Включить (открыть)",
		"Выключить (закрыть)",
		"Телепорт",
		"Выход"
	};

	public static string[] triggerConditionActivateName = new string[]
	{
		"Игрок рядом",
		"Игрок нажал",
		"Получен урон"
	};

	public static string[] triggerNeedKeyName = new string[]
	{
		"Красный ключ",
		"Зелёный ключ",
		"Синий ключ",
		"Золотой ключ"
	};

	public static string[] gameItemsNames = new string[]
	{
		"Факел",
		"Ступеньки К",
		"Ступеньки Д",
		"Лестница",
		"Дверь",
		"Стальная дверь",
		"Стальная дверь 2",
		"Двойная д. дверь",
		"Двойная с. дверь",
		"Ворота",
		"Светильник",
		"Люстра",
		"Красный шар",
		"Зелёный шар",
		"Синий шар",
		"Бочка",
		"Железная бочка",
		"Железная бочка 2",
		"Гроб",
		"Забор",
		"Ограда",
		"Ограда 2",
		"Картина",
		"Картина 2",
		"Скамейка",
		"Скамейка 2",
		"Грибы",
		"Папоротник",
		"Ромашки",
		"Трава",
		"Цветочки",
		"Стол 1",
		"Стол 2",
		"Стул",
		"Сундук",
		"Факел",
		"Куб-лампа(бел)",
		"Куб-лампа(син)",
		"Куб-лампа(зел)",
		"Респаун",
		"Респаун Красных",
		"Респаун Синих",
		"Респаун Зелёных",
		"Респаун Золотых",
		"Респаун патронов",
		"Респаун гильз",
		"Респаун ракет",
		"Респаун энергии",
		"Респаун аптечки",
		"Респаун брони",
		"Респаун ружъя",
		"Респаун обреза",
		"Респаун УЗИ",
		"Респаун автомата",
		"Респаун базуки",
		"Респаун ПТУРС",
		"Респаун зомби",
		"Кнопка",
		"Рычаг",
		"Рычаг на стену",
		"Телепортатор",
		"Невидимый выключатель",
		"Респаун зомби",
		"Респаун агента",
		"Респаун солдата",
		"Респаун шахтёра",
		"Респаун демона",
		"Спрятанный предмет",
		"Куб-лампа(жёл)",
		"Куб-лампа(кр)",
		"Кровать",
		"Стальная лестница",
		"Веревка",
		"Стальные ступеньки",
		"Елочка",
		"Снежная елочка",
		"Большая елочка",
		"Респаун транспорта",
		"Трамплин 3х1",
		"Трамплин 3х2",
		"Трамплин 3х3",
		"Респаун турели",
		"Респаун Банши",
		"Красный флаг",
		"Синий флаг",
		"Зелёный флаг",
		"Золотой флаг",
		"Респаун снайперки",
		"Респаун выжигателя",
		"Точка доминирования",
		"Респаун БТР",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		"Ослепляющая граната",
		"Взрыв пакет",
		"Зелёное дерево",
		"Синее дерево",
		"Осеннее дерево",
		"Вырастить стену",
		"Аптечка и броня",
		string.Empty,
		"Запас патронов",
		"Динамит",
		"Мина",
		"Живая вода",
		"Дверь вертикальная",
		"Дверь горизонтальная",
		"Лифтовая шахта",
		"Силовое поле",
		"Обойма патронов",
		"Обойма гильз",
		"Провод",
		"Обойма ракет",
		"Обойма энергии",
		"Мега-запас"
	};

	public static string[] gameItemsDesc = new string[]
	{
		"Освещает тёмные места",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		"Огромные ворота для замков",
		"Электрический фонарь на стену",
		"Свечная люстра для потолка",
		"Красиво освещает тёмные места",
		"Красиво освещает тёмные места",
		"Красиво освещает тёмные места",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		"Факел в землю, освещение больших площадей",
		"Для встраивания в здания",
		"Для встраивания в здания",
		"Для встраивания в здания",
		"Респаун в некомандных режимах.",
		"Респаун красной команды",
		"Респаун синей команды",
		"Респаун зелёной команды",
		"Респаун золотой команды",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Появляются в бою через некоторое время",
		"Монстры появляются в режиме Выживания",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		"Спрячьте предмет, пусть другие поищут",
		"Для встраивания в здания",
		"Для встраивания в здания",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		"Светится гирляндами",
		"Светится гирляндами",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		"Взрыв ослепляет противника",
		"Взрывается при броске",
		"Вырастает зелёное дерево случайного размера",
		"Вырастает синее дерево случайного размера",
		"Вырастает оранжевое дерево случайного размера",
		"Достраивает 4 куба на выбранную грань",
		"Восстанавливает здоровье и броню",
		string.Empty,
		"Восстанавливает патроны и гильзы",
		"Взрывается через 5 секунд",
		"Взрывается при приближении",
		"Воскрешает персонажа в режиме выживания",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		"300патронов, 100гильз, 50ракет, 300энергии"
	};

	public static string[] specItemsName = new string[]
	{
		"Ракетный ранец",
		"Русский танец",
		"Крейзи танец",
		"Давай, детка!",
		"Акробат",
		"Ееее!",
		"Победа!",
		"Провал...",
		"За мной!"
	};

	public static string[] specItemsDesc = new string[]
	{
		"Будет включаться при прыжке",
		"Танцевать - русск. Р (англ H)",
		"Танцевать - русск. Р (англ H)",
		"Танцевать - русск. Р (англ H)",
		"Танцевать - русск. Р (англ H)",
		"Танцевать - русск. Р (англ H)",
		"Танцевать - русск. Р (англ H)",
		"Танцевать - русск. Р (англ H)",
		"Танцевать - русск. Р (англ H)"
	};

	public static string move_learn = "Выучить";

	public static string[] RankName = new string[]
	{
		"Новобранец",
		"Младший рекрут",
		"Рекрут",
		"Старший рекрут",
		"Рекрут 1 класса",
		"Солдат",
		"Рядовой рекрут",
		"Рядовой 2 класса",
		"Рядовой",
		"Рядовой 1 класса",
		"Специалист",
		"Старший специалист",
		"Специалист 1 класса",
		"Разведчик",
		"Младший капрал",
		"Капрал",
		"Командир",
		"Сержант 3 класса",
		"Сержант 2 класса",
		"Сержант 1 класса",
		"Коммандор-сержант 3кл.",
		"Коммандор-сержант 2кл",
		"Коммандор-сержант 1кл",
		"Коммандор",
		"Младший офицер",
		"Старший офицер",
		"Младший лейтенант",
		"Второй лейтенант",
		"Лейтенант",
		"Капитан",
		"Капитан разведки",
		"Капитан спецназа",
		"Подполковник",
		"Полковник",
		"Генерал",
		"Генерал Армии",
		"Главнокомандующий",
		"Маршал",
		"Капитан отряда",
		"Майор отряда",
		"Генерал-лейтенант",
		"Полковник отряда",
		"Генерал-полковник",
		"Маршал отряда",
		"Мститель",
		"Безумец"
	};

	public static string[] findPrefabsNames = new string[]
	{
		"Секретный документ",
		"Уран"
	};

	public static string[] newMapTypeName = new string[]
	{
		"Залив",
		"Остров",
		"Плоская",
		"Река",
		"Развилка",
		"Большая плоская"
	};

	public static string[] advices = new string[]
	{
		"Цветные китайские шары светятся просто волшебно в темноте!",
		"Ракета из ПТУРС управляется мышкой.",
		"Одежда даёт  бонусы к характеристикам!",
		"Не забудьте купить патронов для боя!",
		"Арбалет стреляет бесконечно.",
		"F10 - не забывайте сохранять свою карту!",
		"F10 - не забывайте сохранять свою карту!",
		"Со статусом V.I.P. все кубы бесплатные",
		"Со статусом V.I.P. быстрее накапливается уровень и деньги",
		"Нажмите V для использования голосового чата",
		"Игра, в которой играет друг, сразу отобразится в сетевой битве справа.",
		"Пройдите по гробу погибшего игрока и он воскреснет",
		"В режиме выживания опыта за фраг получается в 10 раз меньше, чем в битве.",
		"Если удалить предмет с карты - он НЕ появится в инвентаре",
		"Не нравится ваша карта, а новый слот не по карману? Перегенерируйте карту!",
		"На 5 день игры Вам дадут бонус - 2 золотых.",
		"Инвентарь(С)-Перс-VIP - будь круче на 2 звезды!",
		"Инвентарь(С)-Перс-VIP - будь круче на 2 звезды!"
	};

	public static string[] gameTypeStr = new string[]
	{
		"Можно все",
		"Строительство",
		"Каждый сам за себя",
		"Выживание",
		"Команды",
		"Миссия",
		"Захват флага",
		"Доминирование"
	};

	public static string[] buildinMapName = new string[]
	{
		"Перекрёстный огонь",
		"Мясной куб",
		"Город",
		"Бассейн",
		"Особняк",
		"Склад",
		"Остров",
		"Зион-Тех",
		"В песках",
		"Зомби-сити",
		"Арена",
		"Бибз",
		"Страйк",
		"Мотель",
		"Мясорубка",
		"Река",
		"Залив",
		"Остров",
		"Плоская",
		"Перекресток",
		"Река",
		"Арена",
		"Две башни",
		"Страйк",
		"Assault Pro",
		"Мотель",
		"Луммокс"
	};

	public static string[] teamName = new string[]
	{
		"Красные",
		"Синие",
		"Зелёные",
		"Золотые"
	};

	public static string[] graphStrs = new string[]
	{
		"Быстрейшая",
		"Быстрая",
		"Простая",
		"Хорошая",
		"Отличная"
	};

	public static string[] weaponNames = new string[]
	{
		"Кирка",
		"Арбалет",
		"Ружьё",
		"Обрез",
		"Узи",
		"Автомат",
		"Базука",
		"ПТУРС",
		"Лазерный меч",
		"Меч",
		"Плазмоган",
		"Снайперка",
		"Миниган",
		"Огнемёт",
		"Бензопила",
		"Бита",
		"Пулемёт",
		"Гранатомёт",
		"Калашников",
		"БФГ",
		"Риппер",
		"Лазер",
		"Пистолет",
		"Плазменная снайперка",
		"Волшебная палочка",
		"Снежная пушка",
		"Плазменный гранатомёт",
		"Плазмомёт",
		"Выжигатель",
		"Спец. пистолет",
		"Тактический дробовик",
		"Бронебойная снайперка",
		"Револьвер",
		"ППШ"
	};

	public static string[] clothesName = new string[]
	{
		string.Empty,
		string.Empty,
		string.Empty,
		"Ковбойская шляпа",
		"Ковбойская шляпа",
		"Ковбойская шляпа",
		"Зелёные боты",
		"Малиновые боты",
		"Гриндерсы",
		"Боевые берцы",
		"Походные берцы",
		"Железные боты",
		"Космоботы",
		"Причёска Бибер",
		"Причёска",
		"Причёска Эмо",
		"Причёска каштан",
		"Причёска блондин",
		"Кепка",
		"Зелёный ирокез",
		"Фиолетовый ирокез",
		"Красный ирокез",
		"Жёлтый ирокез",
		"Афро",
		"Причёска Рэмбо",
		"Железный шлем",
		"Каска",
		"Шлем",
		"Шлем Вейдера",
		"Шлем Сталкера",
		"Броня Сталкера",
		"Броня Вейдера",
		"Бронежилет",
		"Железная броня",
		"Лента патронов",
		"Золотая цепь",
		"Ранец Сталкера",
		"Рюкзак",
		"Портфель",
		"Наплечники сталкера",
		"Наплечники Вейдера",
		"Железные наплечники",
		"Наручи сталкера",
		"Наручный планшет",
		"Железный наруч",
		"Напульсник",
		"Часы",
		"Голубая причёска",
		"Женский рюкзак",
		"Маска Резака",
		"Маска Гая Фокса",
		"Колдовской колпак",
		"Колдовской колпак",
		"Тыква",
		"Очки",
		"Каска",
		"Фуражка",
		"Шапка",
		"Экзо-броня",
		"Экзо-боты",
		"Экзо-шлем",
		"Экзо-щитки",
		"Экзо-броня",
		"Экзо-боты",
		"Экзо-шлем",
		"Экзо-щитки"
	};

	public static string[] skinName = new string[]
	{
		"Стандарт",
		"Друган",
		"Афро",
		"Сталкер",
		"Солдат",
		"Рэмбо",
		"Железный человек",
		"Вейдер",
		"Агент",
		"Девочка",
		"Коп",
		"Дед Мороз",
		"Экзо"
	};

	public static string[] bulletsNames = new string[]
	{
		"Патроны 5mm",
		"Гильзы",
		"Ракеты",
		"Энергия",
		"Плазма",
		"Патроны 7.62",
		"Уран",
		"Ракеты ПТУРС",
		"Гранаты",
		"Патроны .300"
	};

	public static string AAS_Position = "Положение";

	public static string AAS_Upper = "Выше";

	public static string AAS_Lower = "Ниже";

	public static string AAS_Far = "Дальше";

	public static string AAS_Near = "Ближе";

	public static string AAS_Right = "Правее";

	public static string AAS_Left = "Левее";

	public static string AAS_Size = "Размеры";

	public static string AAS_Higher = "Выше";

	public static string AAS_Smaller = "Ниже";

	public static string AAS_Longer = "Длиннее";

	public static string AAS_Shorter = "Короче";

	public static string AAS_Wider = "Шире";

	public static string AAS_Uje = "Уже";

	public static string AAS_Opening_for_N_sec = "Открывается за";

	public static string sec = "сек";

	public static string AAS_rotation = "Поворот";

	public static string save = "Сохранить";

	public static string BCS_cant_save_if_joined = "---Свою карту нельзя сохранить, если Вы присоединились к игре!---";

	public static string BCS_only_owner_can_save = "---Только владелец карты может её сохранить!---";

	public static string BCS_cant_save_in_battle = "---Карты нельзя сохранять в бою!---";

	public static string BCS_end_round = "Конец раунда";

	public static string[] BCS_levelUpStrs = new string[]
	{
		"УРА! УРА! УРА!!!",
		"ТАК ДЕРЖАТЬ!",
		"ОТЛИЧНО!",
		"ВЕЛИКОЛЕПНО!",
		"С ПОВЫШЕНИЕМ!"
	};

	public static string BCS_endGame_timeout = "ВРЕМЯ ВЫШЛО!";

	public static string BCS_endGame_ban = "ВАС ЗАБАНИЛИ!";

	public static string BCS_endGame_noSuvivours = "Никто не выжил...";

	public static string BCS_endGame_gameOver = "ИГРА ЗАВЕРШЕНА!";

	public static string BCS_endGame_lostConnection = "Потеряно соединение...";

	public static string BCS_endGame_tryAgain = "Попробуй ещё раз!";

	public static string BCS_ban_from_server = "--- Вас выгнали с сервера ---";

	public static string BCS_disable_miscrophone = "Отключить микрофон";

	public static string BCS_to_speak_press = "Чтобы говорить нажмите V";

	public static string BCS_enable_microphone = "Включить микрофон";

	public static string BCS_then_press_allow = "Затем нажмите Allow";

	public static string BCS_get_VIP_to_speak = "Получить статус V.I.P.\nчтобы использовать\nмикрофон";

	public static string BCS_start_test = "Начать тест карты";

	public static string BCS_resume_game = "Продолжить игру (`)";

	public static string BCS_save_map = "Сохранить карту (F10)";

	public static string BCS_inventory_shop = "Инвентарь/магазин (C)";

	public static string BCS_exit_from_game = "Выйти из игры";

	public static string BCS_allowed = "РАЗРЕШЕНО";

	public static string BCS_notallowed = "ЗАПРЕЩЕНО";

	public static string BCS_noobs_to_build = "Новым игрокам строить";

	public static string BCS_forbidBuild = "Запретить строить";

	public static string BCS_allowBuild = "Разрешить строить";

	public static string BCS_ban = "БАН";

	public static string BCS_end_test = "Закончить тест карты";

	public static string BCS_name = "Ник";

	public static string BCS_frags = "Фрагов";

	public static string BCS_deathes = "Смертей";

	public static string BCS_wait_while_saving = "Подождите, карта сохраняется...";

	public static string BCS_dont_forget_save_map = "НЕ ЗАБУДЬТЕ\nСОХРАНИТЬ\nКАРТУ!";

	public static string BCS_save_and_exit = "СОХРАНИТЬ И ВЫЙТИ";

	public static string BCS_dont_save_exit = "Не сохранять. Выйти.";

	public static string BCS_new_level = "Новый уровень!";

	public static string BCS_new_rang = "Ваш новый ранг";

	public static string next = "Дальше";

	public static string BCS_exp = "Опыта";

	public static string BCS_time = "Время";

	public static string BCS_frags_per_minute = "Фрагов в минуту";

	public static string BCS_earned = "За бой заработано";

	public static string BCS_choose_team = "ВЫБЕРИТЕ КОМАНДУ";

	public static string score = "Счёт";

	public static string BCS_enter = "Вступить";

	public static string BCS_loading = "Загружаем...";

	public static string BCS_advice = "Совет";

	public static string BCS_speak_then_release = "Говорите, потом\nотпустите V";

	public static string BCS_speaker_disabled = "---Микрофон выключен. Нажмите Esc(`) ---";

	public static string howto_play = "F1 - как играть?";

	public static string howto_menu = "Esc(`) - меню";

	public static string howto_close = "F1 - закрыть справку";

	public static string howto_walk = "Ходить - ASDW";

	public static string howto_jump = "Прыжок - пробел";

	public static string howto_placeCube1 = "Поставить куб/предмет";

	public static string howto_placeCube2 = "правая кнопка мыши";

	public static string howto_removeCube1 = "Убрать куб";

	public static string howto_removeCube2 = "левая кнопка мыши";

	public static string howto_inventory = "C - инвентарь";

	public static string howto_chooseItem = "1,2,3... - выбор ячейки";

	public static string howto_savemap = "F10 - сохранить карту";

	public static string howto_chat = "Enter - чат";

	public static string howto_voiceChat = "V - голосовой чат";

	public static string howto_fullscreen = "F12 - полный экран";

	public static string howto_options = "O - настройки";

	public static string speedHackDetected = "Обнаружена попытка читерства - SpeedHack";

	public static string[] strTutor = new string[]
	{
		"Добро пожаловать на полигон, солдат! Твоё обучение будет проходить в обстановке, близкой к боевой! Итак, задание - найти проход к заброшенной лаборатории! Осмотрись, и найди для начала выход из этого грота!",
		"Кажется, это чёрный вход. Эта дверь ведёт к лаборатории. Нажми на кнопку, чтобы открыть дверь.",
		"Столб пыли в конце комнаты - это секретная разработка антигравитационного лифта! Чтобы включить его - нажми кнопку слева от лифта.",
		"Молодец! Мне нравится, как ты справляешься! В конце этого коридора, должна быть дверь - похоже, мы дошли до лаборатории!",
		"Замри! Видишь - в соседней комнате стоит враг - зомби. Возьми кирку и уничтожь врага! (нажмите С)",
		"Боец, ты не перестаёшь меня удивлять! Дальше будет веселее.",
		"Теперь нужно освоить более мощное оружие. Возьми этот автомат.",
		"Боец, не забудь про патроны! Ты можешь в любой момент игры купить патронов с базы, нажав R на клавиатуре.",
		"Базука... А за дверью толпа зомби.. Ты знаешь, что делать...",
		"Отлично, боец! Давай в телепорт, ты великолепно себя показал!",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		"А вот и твой мир! Давай, я расскажу, как в нём строить.",
		"Нажми C и выбери любой куб\nЗатем построй 5 кубов в любом месте (правая кнопка мыши)",
		"Молодец! Теперь разрушь 5 любых кубов (левая кнопка мыши)",
		"Молодец! Смотри, наступила ночь! Нужно больше света!",
		"Нажми С - Предметы - купи ФАКЕЛ и поставь на карту",
		"Отлично! Осталось сохранить твою карту! Нажми F10.",
		"Теперь можно построить все что угодно! Желаем приятной стройки!",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty
	};

	public static string[] miniStrTutor = new string[]
	{
		"Найди туннель в пещере",
		"Нажать на рычаг, чтобы открыть дверь",
		"Нажать кнопку, чтобы включить лифт",
		"Дойти до лаборатории",
		"Уничтожить зомби киркой",
		"Подняться по ступенькам на поверхность",
		"Взять автомат",
		"Нажать R и купить патронов",
		"Уничтожить всех зомби",
		"Шагнуть в телепорт",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		"Сохраните карту - F10",
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty
	};

	public static string tutor_need_reload = "Нажмите R, чтобы использовать обойму";

	public static string tutor_reload = "R - перезарядка";

	public static string tutor_choose_weapons = "Выберите раздел 'Оружие'";

	public static string tutor_choose_bullets = "Выберите раздел 'Патроны'";

	public static string tutor_buy_ammo = "Купите обойму";

	public static string tutor_close_inventory = "Закройте инвентарь (С или крестик)";

	public static string tutor_kill_monsters = "Откройте дверь и уничтожьте монстров";

	public static string tutor_placeCube = "Поставьте куб правой кнопкой мыши";

	public static string tutor_placeMoreCubes = "Поставьте ещё кубов";

	public static string tutor_open_inventory = "Нажмите C, чтобы открыть инвентарь";

	public static string tutor_choose_cubes = "Выберите раздел 'Кубы'";

	public static string tutor_choose_nature = "Выберите раздел 'Природные'";

	public static string tutor_choose_any_cube = "Выберите любой куб";

	public static string tutor_place_cube_inventar = "Положите куб в быстрый инвентарь";

	public static string tutor_remove_cube = "Разрушьте куб левой кнопкой мыши";

	public static string tutor_remove_cubes_more = "Разрушьте ещё кубов";

	public static string tutor_place_fire = "Поставьте факел правой кнопкой мыши";

	public static string tutor_choose_decor = "Выберите раздел 'Декорации'";

	public static string tutor_choose_light = "Выберите раздел 'Свет'";

	public static string tutor_choose_fire = "Выберите факел";

	public static string tutor_buy_one_fire = "Купите 1 факел";

	public static string tutor_place_fire_inventar = "Положите факел в быстрый инвентарь";

	public static string tutor_kill_zombie_and_go = "Убейте зомби и пройдите дальше в дверь";

	public static string tutor_choose_axe = "Выберите кирку";

	public static string tutor_buy_axe_forever = "Купите кирку навсегда";

	public static string tutor_place_axe_inventar = "Положите кирку в быстрый инвентарь";

	public static string short_name = "--- Имя должно быть длиннее 3 символов ---";

	public static string rang_table = "Таблица рангов";

	public static string params_health = "Макс. здоровье";

	public static string params_armor = "Макс. броня";

	public static string param_speed = "Скорость";

	public static string param_jump = "Прыжок";

	public static string param_defend = "Стойкость";

	public static string vip_not_enougth = "VIP - Не хватает монет";

	public static string vip_activated = "Статус V.I.P. активирован.";

	public static string bank_name = "БАНК";

	public static string inventory_name = "ИНВЕНТАРЬ";

	public static string maximum = "Максимум";

	public static string hello_chiter = "Привет, читер. Попытка записана в базу. Больше так не делай.";

	public static string server_error = "На сервере произошел сбой.";

	public static string ban_cheater = "Привет, читер. Тебя забанили. Если ты не виноват обратись к администрации группы.";

	public static string loading_data = "Загрузка данных.";

	public static string[] newGameMapTypeStr = new string[]
	{
		"Мои Карты",
		"Встроенные карты"
	};

	public static string[] filterGameTypeStr = new string[]
	{
		"Любой тип битвы",
		"Каждый сам за себя",
		"Строительство",
		"Выживание",
		"Команды",
		"Захват флага",
		"Доминирование"
	};

	public static string[] mapCanBreakStr = new string[]
	{
		"Неважно",
		"Без разрушений",
		"С разрушениями"
	};

	public static string[] mapDayLightStr = new string[]
	{
		"Неважно",
		"День",
		"Смена дня и ночи",
		"Ночь"
	};

	public static string[] serverTypeStr = new string[]
	{
		"Сервера",
		"Мои\nкарты",
		"Любая\nкарта"
	};

	public static string begin_game = "Начать игру";

	public static string any_map = "Любая карта";

	public static string map = "Карта";

	public static string self_map = "Карта игрока";

	public static string wrong_password = "НЕВЕРНЫЙ ПАРОЛЬ! (пароль пишется внизу справа)";

	public static string[] respawnTimeStr = new string[]
	{
		"10 сек",
		"30 сек",
		"1 мин",
		"2 мин",
		"5 мин",
		"10 мин",
		"30 мин",
		"нет"
	};

	public static string monster_options = "Настройка точки возрождения монстра";

	public static string monster_type = "Тип монстра";

	public static string ressurection_time = "Время возрождения";

	public static string error_empty_map = "Пустая карта - попробуйте другую игру";

	public static string player_can_build_now = "теперь может строить";

	public static string player_cant_build_now = "запрещено строить!";

	public static string opt_name = "Настройки";

	public static string opt_graph = "Графика";

	public static string opt_worse = "Хуже";

	public static string opt_better = "Лучше";

	public static string opt_sound = "Звук";

	public static string opt_silent = "..aa..";

	public static string opt_louder = "!!AA!!";

	public static string opt_empty_screen = "Чистый\nэкран";

	public static string opt_smooth_follow = "Сглаженное\nдвижение";

	public static string opt_close = "Закрыть(O)";

	public static string connect_failed = "Не удалось присоединиться...";

	public static string create_room_failed = "Не удалось создать комнату...";

	public static string put_on_items = "--- Поставьте на выключатели ---";

	public static string cube_occupied = "--- Этот куб занят ---";

	public static string to_delete_press = "Нажмите (DEL) чтобы удалить";

	public static string to_move_press = "Нажмите (E) чтобы переместить";

	public static string to_activate_press = "Нажмите (F) чтобы активировать";

	public static string to_rotate_press = "Нажмите (R) чтобы повернуть";

	public static string to_edit_press = "Нажмите (T) чтобы настроить";

	public static string cant_build_ask_admin = "Стройка запрещена. Обратитесь к Администратору карты.";

	public static string cant_change_world = "В этом режиме нельзя изменять мир.";

	public static string not_enougth_cubes = "Больше нет кубов. В бой!";

	public static string beside_world = "--- За пределами карты ---";

	public static string place_on_cube_side = "--- поставьте на боковую сторону куба ---";

	public static string place_on_ceil = "--- устанавливается на потолок ---";

	public static string cant_already_remove = "Ломать больше нельзя. В бой!";

	public static string find_item_choose_type = "Выберите тип предмета";

	public static string find_item_type = "Тип предмета";

	public static string apply = "ПРИМЕНИТЬ";

	public static string _for = "за";

	public static string ps_choose_new_item_place = "Выберите новое место для установки предмета.";

	public static string ps_press_for_respawn = "Нажмите прыжок(пробел) для респауна";

	public static string ps_you_dead_try_again = "Вы погибли попробуйте начать миссию заново";

	public static string ps_survival_dead = "Возрождение в начале волны, либо если Вас спасут.";

	public static string ps_survival_if_VIP = "Если у Вас статус V.I.P., Вы возродитесь через 30с.";

	public static string ps_use_vita_water = "Быстрое возрождение - Живая Вода\n(Инвентарь(С)-Снаряжение-Боевые-Живая Вода)";

	public static string ps_press_for_use_vita_water = "Для возрождения нажмите X - вас восстановит Живая Вода";

	public static string ps_before_respawn_secs = "До возрождения осталось";

	public static string ps_health_and_armor_restored = "Здоровье и броня восстановлены";

	public static string ss_loading = "Загрузка";

	public static string social_tell_friends = "Рассказать друзьям";

	public static string social_menu = "Добавить в меню";

	public static string social_invite_friends = "Пригласить друзей";

	public static string social_group = "Вступить в группу";

	public static string social_invite_num_friends = "Пригласить 3x друзей";

	public static string social_tell_about_game = "Рассказать о игре";

	public static string social_install_game = "Установить приложение";

	public static string trig_options = "Настройка выключателя";

	public static string trig_type = "Тип выключателя";

	public static string trig_triggered_if = "Срабатывает, если:";

	public static string trig_need_for_triggering = "Для срабатывания нужен:";

	public static string trig_mission_exit = "Выход из миссии!";

	public static string wire_put_on_switch = "Провод можно ставить только на выключатель!";

	public static string wire_options = "Настройка провода";

	public static string wire_signal_delay = "Задержка сигнала";

	public static string wire_connected_with = "Провод подсоединён к";

	public static string wire_not_connected = "не подсоединён";

	public static string wire_connected_moveable_cubes = "Подвижные кубы";

	public static string wire_connected_switch = "Выключатель";

	public static string wire_connected_item = "Предмет";

	public static string wire_connected_coords = "Координаты";

	public static string wire_connect_to = "Поключить провод";

	public static string wire_choose_connect_to = "Выберите место, куда подключить провод. (Для телепорта - просто куб, для подключения - подвижные кубы, предметы, выключатели.";

	public static string plazm_grenade_gun = "Плазменный гранатомет";

	public static string seconds = "Секунд";

	public static string secondu = "Секунду";

	public static string secondy = "Секунды";

	public static string minutes = "Минут";

	public static string minutu = "Минуту";

	public static string minuty = "Минуты";

	public static string monsters = "Монстров";

	public static string monstra = "Монстра";

	public static string viral_title = "Выполни задание и получи награду";

	public static string viral_bonus = "Награда";

	public static string player_exit = "вышел из игры";

	public static string player_joined = "присоединился к игре";

	public static string need_bullets_nothing = "ничего";

	public static string need = "Нужен";

	public static string put_on = "Одеть";

	public static string activities_title = "Движения";

	public static string[] offer_text = new string[]
	{
		"Дамы и господа, торопитесь только сегодня действует уникальная распродажа. Скидки в магазине на указанные товары 50%",
		"Выгодная цена на золото",
		"Больше баксов и опыта на миссиях"
	};

	public static string monster_health_mult = "Множитель здоровья";

	public static string monster_damage_mult = "Множитель урона";

	public static string monster_state = "С этими параметрами у монстра новый статус";

	public static string boss = "БОСС";

	public static string newYearBoss_mission_adv = "Проверь себя! Новогодний БОСС!";

	public static string newYearBoss_mission_need_level = "Необходим 5 уровень.";

	public static string transport_options = "Настройка точки возрождения транспорта";

	public static string transport_type = "Тип транспорта";

	public static string[] transportName = new string[]
	{
		"Квадроцикл",
		"Турель",
		"Банши",
		"БТР"
	};

	public static string to_drive_press = "Нажмите (E), чтобы  управлять";

	public static string no_place_to_drive = "--- Все места заняты! ---";

	public static string press_to_end_drive = "Нажмите (X), чтобы выйти";

	public static string[] mission_name = new string[]
	{
		string.Empty,
		"Обучение",
		string.Empty,
		string.Empty,
		"Защита деревни",
		"Поиск чертежей",
		"Последняя оборона",
		"Зачистка парка",
		string.Empty,
		string.Empty,
		"Радиоактивность",
		"Вторая волна",
		"База Альфа",
		"Вектор",
		"Танковый Завод",
		"Фабрика страха",
		"Тайник мертвеца",
		"Блекджек",
		"Контроль",
		"Тагил",
		"Плоскость",
		"Крамп",
		"Чайна-Таун",
		"Осада",
		"Крест",
		string.Empty,
		"Выжить!",
		"Уничтожить!",
		"Найти!",
		"Собрать!",
		"Выйти!",
		"Зачистить!"
	};

	public static string shots_per_sec = "Выстр. в сек: ";

	public static string requireBullets = "Нужно: ";

	public static string for_fullScreen_press = "Полноэкранный режим - F12";

	public static string pieses = "Кол-во";

	public static string Buy = "Купить";

	public static string social_folow_fanpage = "Подписаться на страницу";

	public static string you_killed = "Вы убили";

	public static string you_was_killed_by = "Вы были убиты";

	public static string killed = "убил";

	public static string to_next_round_last = "До следующего раунда";

	public static string shooter_end_battle = "Закончить битву";

	public static string[] winner_place = new string[]
	{
		"1 место",
		"2 место",
		"3 место"
	};

	public static string dead_by_nature = "погиб";

	public static string dead_himself = "самоуничтожился";

	public static string dead_by_zombie = "съели";

	public static string he_saved = "спас";

	public static string you_saved = "Вы спасли";

	public static string he_saved_you = "Вас спас";

	public static string headshot = "ХЭДШОТ";

	public static string offer_title = "Акция";

	public static string sale_title = "Распродажа";

	public static string offer_open_shop = "В магазин";

	public static string offer_open_bank = "В банк";

	public static string howto_tab = "TAB - очки игроков";

	public static string you_have_flag = "У вас вражеский флаг! Отнесите его на свою базу!";

	public static string your_flag_captured = "Ваш флаг украден! Верните его на базу!";

	public static string flag_has_been_captured = "успешно захватывают флаг!";

	public static string[] flag_color_name = new string[]
	{
		"Красный",
		"Синий",
		"Зелёный",
		"Золотой"
	};

	public static string takes_flag = "взял";

	public static string dropped_flag = "бросил";

	public static string flag = "флаг";

	public static string BCS_Score = "Счёт";

	public static string cant_take_flag_no_players = "Вы НЕ МОЖЕТЕ взять флаг, так как в этой команде нет игроков";

	public static string you_returned_flag = "Вы вернули флаг на базу";

	public static string[] bonusName = new string[]
	{
		"ПОДРЫВ",
		"ДВОЙНОЙ ПОДРЫВ",
		"ВЗРЫВ",
		"ДВОЙНОЙ ВЗРЫВ",
		"2 ФРАГА",
		"5 ФРАГОВ",
		"7 ФРАГОВ",
		"10 ФРАГОВ",
		"ХЭДШОТ",
		"ХЭДШОТ 3",
		"ХЭДШОТ 5",
		"СПАСАТЕЛЬ",
		"5 СПАСЁННЫХ",
		"СВЯТОЙ",
		"РУКОПАШКА",
		"РУКОПАШКА 3",
		"САМОУБИЙЦА",
		"КОМАНДА-ПОБЕДИТЕЛЬ",
		"1 МЕСТО",
		"2 МЕСТО",
		"3 МЕСТО",
		"ЗАХВАТ ФЛАГА",
		"МИССИЯ ПРОЙДЕНА",
		"5 зомби",
		"20 зомби",
		"50 зомби",
		"3 демона",
		"10 демонов",
		"3 зомби взрывом",
		"20 зомби взрывом",
		"50 зомби взрывом",
		"Уничтожение техники",
		"3 техники",
		"1 волна",
		"3 волны",
		"5 волн",
		"10 волн",
		"50 кубов",
		"150 кубов",
		"5 предметов",
		"20 предметов",
		"Механизм"
	};

	public static string your_team_is = "Ваша команда - ";

	public static string frags_killed = "Фрагов";

	public static string need_level = "Нужен {0} уровень";

	public static string[] weapon_upgrade_name = new string[]
	{
		"Урон",
		"Разброс",
		"Скорострельность",
		"Обойма",
		"Начальный запас"
	};

	public static string[] DecorTypesNew = new string[]
	{
		"Свет",
		"Мебель",
		"Двери",
		"Лестницы",
		"Растения",
		"Декор",
		"Дороги"
	};

	public static string[] SpawnerTypes = new string[]
	{
		"Оружия",
		"Монстры",
		"Игровые",
		"Другое"
	};

	public static string no_weapon_upgrade = "Можно прокачать только оружие купленное навсегда";

	public static string needParamsToBuy1 = "Требуется";

	public static string needParamsToBuyLevel = "уровень";

	public static string needParamsUpgradeNow = "Прокачать ВСЁ сейчас за";

	public static string Unlock = "Разблокировать за";

	public static string Upgrade = "Улучшить";

	private static Dictionary<string, string> _hash = new Dictionary<string, string>();

	private static Type _tt = typeof(Localize);
}
