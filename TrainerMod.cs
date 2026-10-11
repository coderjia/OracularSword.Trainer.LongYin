using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(OracularSword.Trainer.LongYin.TrainerMod), "修改器", "1.8.8", "local")]

namespace OracularSword.Trainer.LongYin
{
    public class TrainerMod : MelonMod
    {
        const int TargetDays = 1;

        static TrainerMod _instance;
        static MelonPreferences_Entry<bool> _buildEnabled;
        static MelonPreferences_Entry<bool> _researchEnabled;
        static MelonPreferences_Entry<bool> _infiniteHp;
        static MelonPreferences_Entry<bool> _infiniteMana;
        static MelonPreferences_Entry<bool> _infiniteStamina;
        static MelonPreferences_Entry<bool> _maxMoveEnabled;
        static MelonPreferences_Entry<bool> _oneHitEnabled;
        static MelonPreferences_Entry<bool> _infiniteAction;
        static MelonPreferences_Entry<bool> _buffChargeEnabled;
        static MelonPreferences_Entry<bool> _noSkillCdEnabled;
        static MelonPreferences_Entry<bool> _equipWeightEnabled;
        static MelonPreferences_Entry<bool> _itemWeightEnabled;
        static MelonPreferences_Entry<bool> _bookWeightEnabled;
        static MelonPreferences_Entry<bool> _dateLockEnabled;
        static MelonPreferences_Entry<bool> _skillExpEnabled;
        static MelonPreferences_Entry<bool> _maxFavorEnabled;
        static MelonPreferences_Entry<bool> _townStateEnabled;
        static MelonPreferences_Entry<bool> _exploreFogEnabled;
        static MelonPreferences_Entry<bool> _exploreTeleportEnabled;
        static MelonPreferences_Entry<bool> _explorePowerLockEnabled;
        static MelonPreferences_Entry<bool> _itemRareLockEnabled;
        static MelonPreferences_Entry<int> _itemRareIndex;
        static MelonPreferences_Entry<bool> _auctionWinEnabled;
        static int _expGainDepth;
        static bool _fillingExp;
        static bool _loggedExpFailure;
        static int _lockYear;
        static int _lockMonth;
        static int _lockDay;
        static bool _dateCaptured;
        static int _playerForceId;
        static int _playerHeroId;
        static bool _hasPlayerForce;
        static int _houseUpgradeDepth;
        static float _timer;
        static bool _loggedWeightFailure;
        static bool _loggedTownFailure;
        static bool _loggedExploreFailure;
        static int _exploreClickFrame = -1;
        static int _teleportDepth;
        static bool _appliedSaved;
        static bool _f8Held;
        static int _hotkeyFrame = -1;
        static bool _panelOpen;
        static int _tab;
        static bool _rareMenuOpen;
        static bool _capturedIgnore;
        static bool _savedIgnore;
        static bool _inputHeld;
        static bool _loggedInputHold;
        static readonly List<Behaviour> _heldBehaviours = new List<Behaviour>();
        static readonly List<bool> _heldUseMouse = new List<bool>();
        static readonly List<bool> _heldUseTouch = new List<bool>();
        static readonly List<Camera> _heldCameras = new List<Camera>();
        static readonly List<int> _heldMasks = new List<int>();
        static bool _readingRealResearchDays;
        static Font _uiFont;
        static GUIStyle _windowStyle;
        static GUIStyle _toggleStyle;
        static GUIStyle _buttonStyle;
        static GUIStyle _labelStyle;
        static GUIStyle _optionStyle;
        static GUIStyle _hitStyle;
        static Texture2D _whiteTex;
        static float _styleScale = -1f;
        const int MaxMoveRange = 99;

        public static bool BuildEnabled { get; private set; }
        public static bool ResearchEnabled { get; private set; }
        public static bool InfiniteHp { get; private set; }
        public static bool InfiniteMana { get; private set; }
        public static bool InfiniteStamina { get; private set; }
        public static bool MaxMoveEnabled { get; private set; }
        public static bool OneHitEnabled { get; private set; }
        public static bool InfiniteAction { get; private set; }
        public static bool BuffChargeEnabled { get; private set; }
        public static bool NoSkillCdEnabled { get; private set; }
        public static bool EquipWeightEnabled { get; private set; }
        public static bool ItemWeightEnabled { get; private set; }
        public static bool BookWeightEnabled { get; private set; }
        public static bool DateLockEnabled { get; private set; }
        public static bool SkillExpEnabled { get; private set; }
        public static bool MaxFavorEnabled { get; private set; }
        public static bool TownStateEnabled { get; private set; }
        public static bool ExploreFogEnabled { get; private set; }
        public static bool ExploreTeleportEnabled { get; private set; }
        public static bool ExplorePowerLockEnabled { get; private set; }
        public static bool ItemRareLockEnabled { get; private set; }
        public static int ItemRareIndex { get; private set; }
        public static bool AuctionWinEnabled { get; private set; }

        public override void OnInitializeMelon()
        {
            _instance = this;
            var category = MelonPreferences.CreateCategory("FastBuild");
            _buildEnabled = category.CreateEntry("Enabled", false);
            _researchEnabled = category.CreateEntry("ResearchEnabled", false);
            bool legacyBattle = category.CreateEntry("BattleEnabled", false).Value;
            _infiniteHp = category.CreateEntry("InfiniteHp", legacyBattle);
            _infiniteMana = category.CreateEntry("InfiniteMana", legacyBattle);
            _infiniteStamina = category.CreateEntry("InfiniteStamina", legacyBattle);
            _maxMoveEnabled = category.CreateEntry("MaxMoveEnabled", legacyBattle);
            _oneHitEnabled = category.CreateEntry("OneHitEnabled", legacyBattle);
            _infiniteAction = category.CreateEntry("InfiniteAction", legacyBattle);
            _buffChargeEnabled = category.CreateEntry("BuffChargeEnabled", legacyBattle);
            _noSkillCdEnabled = category.CreateEntry("NoSkillCdEnabled", legacyBattle);
            _equipWeightEnabled = category.CreateEntry("EquipWeightEnabled", false);
            _itemWeightEnabled = category.CreateEntry("ItemWeightEnabled", false);
            _bookWeightEnabled = category.CreateEntry("BookWeightEnabled", false);
            _dateLockEnabled = category.CreateEntry("DateLockEnabled", false);
            _skillExpEnabled = category.CreateEntry("SkillExpEnabled", false);
            _maxFavorEnabled = category.CreateEntry("MaxFavorEnabled", false);
            _townStateEnabled = category.CreateEntry("TownStateEnabled", false);
            _exploreFogEnabled = category.CreateEntry("ExploreFogEnabled", false);
            _exploreTeleportEnabled = category.CreateEntry("ExploreTeleportEnabled", false);
            _explorePowerLockEnabled = category.CreateEntry("ExplorePowerLockEnabled", false);
            _itemRareLockEnabled = category.CreateEntry("ItemRareLockEnabled", false);
            _itemRareIndex = category.CreateEntry("ItemRareIndex", 5);
            _auctionWinEnabled = category.CreateEntry("AuctionWinEnabled", false);
            BuildEnabled = _buildEnabled.Value;
            ResearchEnabled = _researchEnabled.Value;
            InfiniteHp = _infiniteHp.Value;
            InfiniteMana = _infiniteMana.Value;
            InfiniteStamina = _infiniteStamina.Value;
            MaxMoveEnabled = _maxMoveEnabled.Value;
            OneHitEnabled = _oneHitEnabled.Value;
            InfiniteAction = _infiniteAction.Value;
            BuffChargeEnabled = _buffChargeEnabled.Value;
            NoSkillCdEnabled = _noSkillCdEnabled.Value;
            EquipWeightEnabled = _equipWeightEnabled.Value;
            ItemWeightEnabled = _itemWeightEnabled.Value;
            BookWeightEnabled = _bookWeightEnabled.Value;
            DateLockEnabled = _dateLockEnabled.Value;
            SkillExpEnabled = _skillExpEnabled.Value;
            MaxFavorEnabled = _maxFavorEnabled.Value;
            TownStateEnabled = _townStateEnabled.Value;
            ExploreFogEnabled = _exploreFogEnabled.Value;
            ExploreTeleportEnabled = _exploreTeleportEnabled.Value;
            ExplorePowerLockEnabled = _explorePowerLockEnabled.Value;
            ItemRareLockEnabled = _itemRareLockEnabled.Value;
            ItemRareIndex = _itemRareIndex.Value;
            AuctionWinEnabled = _auctionWinEnabled.Value;

            var harmony = new HarmonyLib.Harmony("oracularsword.trainer.longyin");
            PatchGetter(harmony, AccessTools.Method(typeof(AreaBuildingData), nameof(AreaBuildingData.GetUpgradeTime)));
            PatchGetter(harmony, AccessTools.Method(typeof(AreaBuildingData), nameof(AreaBuildingData.GetBuildTime)));
            PatchGetter(harmony, AccessTools.Method(typeof(AreaBuildingData), nameof(AreaBuildingData.GetDestroyTime)));
            PatchGetter(harmony, AccessTools.Method(typeof(AreaBuildingData), nameof(AreaBuildingData.GetMoveTime)));
            PatchGetter(harmony, AccessTools.Method(typeof(AreaBuildingDataBase), nameof(AreaBuildingDataBase.GetBuildTime)));
            PatchGetter(harmony, AccessTools.Method(typeof(AreaRoadData), nameof(AreaRoadData.GetUpgradeTime)));
            PatchGetter(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.GetUpgradeDefenceLvDay)));

            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.ChangeDay), Type.EmptyTypes), nameof(BeforeDayPasses), nameof(RestoreLockedDate));
            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.ChangeDay), new[] { typeof(int) }), nameof(BeforeTimedDayPass), nameof(RestoreLockedDate));
            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.ChangeDayDirect)), nameof(BeforeTimedDayPass), nameof(RestoreLockedDate));
            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.ChangeMonth), Type.EmptyTypes), nameof(BeforeDayPasses), nameof(RestoreLockedDate));
            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.ChangeMonthDirect)), nameof(BeforeDayPasses), nameof(RestoreLockedDate));
            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.ChangeYear), Type.EmptyTypes), nameof(BeforeDayPasses), nameof(RestoreLockedDate));
            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.ChangeYearDirect)), nameof(BeforeDayPasses), nameof(RestoreLockedDate));
            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.ChangeHour)), null, nameof(RestoreLockedDate));

            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.BuildingBuildStart)), null, nameof(AfterConstructionStarted));
            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.BuildingUpgradeStart)), null, nameof(AfterConstructionStarted));
            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.ObstacleDestroyStart)), null, nameof(AfterConstructionStarted));
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.StartUpgradeDefenceLv)), null, nameof(AfterConstructionStarted));
            Patch(harmony, AccessTools.Method(typeof(BuildingUIController), nameof(BuildingUIController.UpgradeCityHouse)), nameof(EnterHouseUpgrade), nameof(LeaveHouseUpgrade));
            Patch(harmony, AccessTools.Method(typeof(PlotController), nameof(PlotController.UpgradeCityHouse)), nameof(EnterHouseUpgrade), nameof(LeaveHouseUpgrade));
            Patch(harmony, AccessTools.Method(typeof(ForceTechLvData), nameof(ForceTechLvData.GetResearchLeftDay)), null, nameof(CapResearchLeftDay));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.GetMoveRange)), null, nameof(MaxAllyMoveRange));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.GetBaseMoveRange)), null, nameof(MaxAllyMoveRange));
            Patch(harmony, AccessTools.Method(typeof(BattleUnit), nameof(BattleUnit.ChangeHp)), nameof(BlockAllyHp), null);
            Patch(harmony, AccessTools.Method(typeof(BattleUnit), nameof(BattleUnit.ChangeTrueHp)), nameof(BlockAllyHp), null);
            Patch(harmony, AccessTools.Method(typeof(BattleUnit), nameof(BattleUnit.ChangeMana)), nameof(BlockAllyMana), null);
            Patch(harmony, AccessTools.Method(typeof(BattleUnit), nameof(BattleUnit.ChangePower)), nameof(BlockAllyStamina), null);
            Patch(harmony, AccessTools.Method(typeof(BattleController), nameof(BattleController.ManageDamage)), nameof(BeforeOneHit), null);
            Patch(harmony, AccessTools.Method(typeof(BattleController), nameof(BattleController.CountBaseDamage)), null, nameof(AfterOneHitDamage));
            Patch(harmony, AccessTools.Method(typeof(KungfuSkillLvData), nameof(KungfuSkillLvData.ChangePower)), null, nameof(KeepBuffCharged));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.ChangeSkillPower)), null, nameof(RefillAllyBuffCharge));
            Patch(harmony, AccessTools.Method(typeof(HeroEquipmentData), nameof(HeroEquipmentData.RecountEquipWeight)), null, nameof(AfterRecountEquip));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.GetEquipmentWeightLv)), null, nameof(ZeroEquipLevel));
            Patch(harmony, AccessTools.Method(typeof(AIController), nameof(AIController.CheckHeroItemWeightBiggerThanMax)), nameof(NotOverweight), null);
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.AddSkillFightExp)), nameof(EnterExpGain), nameof(AfterSkillExp));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.AddSkillBookExp)), nameof(EnterExpGain), nameof(AfterSkillExp));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.BattleChangeSkillFightExp)), nameof(EnterExpGain), nameof(AfterSkillExp));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.AutoGetFightExp), Type.EmptyTypes), nameof(EnterAutoExp), nameof(LeaveAutoExp));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.AutoGetFightExp), new[] { typeof(float) }), nameof(EnterAutoExp), nameof(LeaveAutoExp));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.ChangeFavor)), nameof(EnterFavor), nameof(LeaveFavor));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.SetMeetFavor)), nameof(EnterMeetFavor), null);
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.SetHeroMeet)), nameof(EnterMeetFavor), null);
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.ChangePeople), new[] { typeof(float) }), nameof(LockTownDelta), null);
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.ChangePeople), new[] { typeof(float), typeof(bool) }), nameof(LockTownDelta), null);
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.ChangeSafe), new[] { typeof(float) }), nameof(LockTownDelta), null);
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.ChangeSafe), new[] { typeof(float), typeof(bool) }), nameof(LockTownDelta), null);
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.ChangeSupport), new[] { typeof(float) }), nameof(LockTownDelta), null);
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.ChangeSupport), new[] { typeof(float), typeof(bool) }), nameof(LockTownDelta), null);
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.ChangeDefence), new[] { typeof(float) }), nameof(LockTownDelta), null);
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.ChangeDefence), new[] { typeof(float), typeof(bool) }), nameof(LockTownDelta), null);
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.ChangeAreaState)), nameof(LockTownAreaState), null);
            Patch(harmony, AccessTools.Method(typeof(AreaData), nameof(AreaData.AreaConquerReduceDefenceLv)), nameof(LockTownDefenceDrop), null);
            Patch(harmony, AccessTools.Method(typeof(ExploreController), nameof(ExploreController.GenerateExplorePanel)), null, nameof(RevealExploreMap));
            Patch(harmony, AccessTools.Method(typeof(ExploreController), nameof(ExploreController.ShowExploreAnimFinished)), null, nameof(RevealExploreMap));
            Patch(harmony, AccessTools.Method(typeof(ExploreTileUnitController), nameof(ExploreTileUnitController.OnClick)), nameof(TeleportExploreClick), null);
            Patch(harmony, AccessTools.Method(typeof(ExploreController), nameof(ExploreController.ExploreTileClicked), new[] { typeof(ExploreTileData) }), nameof(TeleportExploreTile), null);
            Patch(harmony, AccessTools.Method(typeof(ExploreController), nameof(ExploreController.ChangeMoveStep), new[] { typeof(int) }), nameof(BlockExplorePowerDrain), null);
            Patch(harmony, AccessTools.Method(typeof(ExploreController), nameof(ExploreController.ChangeMoveStep), new[] { typeof(int), typeof(bool) }), nameof(BlockExplorePowerDrain), null);
            Patch(harmony, AccessTools.Method(typeof(ExploreController), nameof(ExploreController.PlayerEnterGrid), new[] { typeof(ExploreTileData), typeof(bool) }), null, nameof(KeepExplorePower));
            Patch(harmony, AccessTools.Method(typeof(ExploreController), nameof(ExploreController.PlayerEnterGrid), new[] { typeof(GameObject), typeof(bool) }), null, nameof(KeepExplorePower));
            Patch(harmony, AccessTools.Method(typeof(ExploreController), nameof(ExploreController.PlayerEnterGrid), new[] { typeof(int), typeof(int), typeof(bool) }), null, nameof(KeepExplorePower));
            Patch(harmony, AccessTools.Method(typeof(ExploreController), nameof(ExploreController.ManageMoveStepLimit)), null, nameof(KeepExplorePower));
            Patch(harmony, AccessTools.Method(typeof(GameController), nameof(GameController.RandomRareLvByBossLv)), null, nameof(ForceRolledRare));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.GetItem), new[] { typeof(ItemData), typeof(bool) }), nameof(BeforeGotItem), nameof(AfterGotItem), nameof(EndGrant));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.GetItem), new[] { typeof(ItemData), typeof(bool), typeof(bool), typeof(int), typeof(bool) }), nameof(BeforeGotItem), nameof(AfterGotItem), nameof(EndGrant));
            Patch(harmony, AccessTools.Method(typeof(HeroData), nameof(HeroData.GetItem), new[] { typeof(ItemListData), typeof(bool), typeof(bool), typeof(int) }), nameof(BeforeGotBundle), nameof(AfterGotBundle), nameof(EndGrant));
            Patch(harmony, AccessTools.Method(typeof(ItemListData), nameof(ItemListData.GetItem), new[] { typeof(ItemData), typeof(bool) }), nameof(BeforeBagItem), nameof(AfterBagItem), nameof(EndGrant));
            Patch(harmony, AccessTools.Method(typeof(ItemListData), nameof(ItemListData.GetItem), new[] { typeof(ItemListData) }), nameof(BeforeBagBundle), nameof(AfterBagBundle), nameof(EndGrant));
            Patch(harmony, AccessTools.Method(typeof(ItemData), nameof(ItemData.RecountRareLv)), null, nameof(KeepGrantedRare));
            Patch(harmony, AccessTools.Method(typeof(AuctionController), nameof(AuctionController.StartAuctionDeal)), nameof(ClaimAuctionWinner), null);
            Patch(harmony, AccessTools.Method(typeof(AuctionController), nameof(AuctionController.AutoFinishAuction)), nameof(ClaimAuctionWinner), nameof(GiveLeftoverLots));
            PatchAuctionDeal(harmony);

            LoggerInstance.Msg("已加载。左上角「修改器」或 F8 打开窗口。");
        }

        public override void OnUpdate()
        {
            PollHotkey();
            HoldGameInput();
            SyncSavedToggles();
            ApplySavedWhenReady();

            if (DateLockEnabled)
                RestoreDate();

            if (NeedsBattleTick())
                ApplyBattle();
            if (ExploreFogEnabled || ExplorePowerLockEnabled)
                ApplyExplore();
            if (_auctionClaim)
            {
                var auction = AuctionNow();
                if (auction == null || auction.auctionStep == AuctionStep.None)
                    _auctionClaim = false;
            }

            bool weightEnabled = EquipWeightEnabled || ItemWeightEnabled || BookWeightEnabled;
            if (!BuildEnabled && !ResearchEnabled && !weightEnabled && !TownStateEnabled)
                return;

            _timer += Time.unscaledDeltaTime;
            if (_timer < 0.4f)
                return;
            _timer = 0f;
            if (BuildEnabled)
                ApplyBuild();
            if (ResearchEnabled)
                ApplyResearch();
            if (weightEnabled)
                ApplyWeight();
            if (TownStateEnabled)
                ApplyTownState();
        }

        public override void OnGUI()
        {
            var keyEvent = Event.current;
            if (keyEvent != null && keyEvent.type == EventType.KeyDown && keyEvent.keyCode == KeyCode.F8)
            {
                TogglePanel();
                keyEvent.Use();
            }
            try
            {
                if (Input.GetKeyDown(KeyCode.F8))
                    TogglePanel();
            }
            catch
            {
            }

            EnsureStyles();
            float scale = UiScale();
            float font = 16f * scale;
            if (!_panelOpen)
            {
                if (GUI.Button(new Rect(12f, 12f, font * 6.6f, font * 2f), "修改器", _buttonStyle))
                    _panelOpen = true;
                return;
            }

            float width = Mathf.Min(font * 32f, Screen.width - 40f);
            float row = font * 2.2f;
            float menu = _tab == 0 && _rareMenuOpen ? GradeNames().Length * row * 0.9f : 0f;
            float height = Mathf.Min(font * 2.8f + row * 2.3f + TabBodyRows() * row + menu + font * 8.2f, Screen.height - 40f);
            var window = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            GUI.Box(window, "修改器", _windowStyle);
            DrawWindow(window, font, row);
            var pointer = Event.current;
            if (pointer != null && (pointer.isMouse || pointer.type == EventType.ScrollWheel))
                pointer.Use();
        }

        static float UiScale()
        {
            return Mathf.Clamp(Screen.height / 1080f, 0.9f, 1.7f);
        }

        static void EnsureStyles()
        {
            float scale = UiScale();
            if (_toggleStyle != null && Mathf.Abs(_styleScale - scale) < 0.02f)
                return;
            _styleScale = scale;
            int fontSize = Mathf.RoundToInt(16f * scale);
            if (_uiFont == null)
            {
                try
                {
                    _uiFont = Font.CreateDynamicFontFromOSFont(new[] { "Microsoft YaHei UI", "Microsoft YaHei", "SimHei" }, fontSize);
                }
                catch
                {
                    _uiFont = GUI.skin.font;
                }
            }

            _windowStyle = new GUIStyle(GUI.skin.window);
            _windowStyle.font = _uiFont;
            _windowStyle.fontSize = fontSize + 2;
            _windowStyle.alignment = TextAnchor.UpperCenter;
            var panel = WindowTex();
            var title = new Color(0.96f, 0.92f, 0.82f, 1f);
            ApplyOpaque(_windowStyle, panel, title);
            _windowStyle.border = new RectOffset(0, 0, 0, 0);
            _toggleStyle = new GUIStyle(GUI.skin.label);
            _toggleStyle.font = _uiFont;
            _toggleStyle.fontSize = fontSize;
            _toggleStyle.alignment = TextAnchor.MiddleLeft;
            _toggleStyle.padding = new RectOffset(0, 0, 0, 0);
            _toggleStyle.normal.textColor = title;
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.font = _uiFont;
            _buttonStyle.fontSize = fontSize;
            _buttonStyle.alignment = TextAnchor.MiddleCenter;
            _buttonStyle.padding = new RectOffset(6, 6, 2, 2);
            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.font = _uiFont;
            _labelStyle.fontSize = fontSize;
            _labelStyle.wordWrap = true;
            _labelStyle.normal.textColor = new Color(0.96f, 0.92f, 0.82f, 1f);
            _optionStyle = _toggleStyle;
            _hitStyle = new GUIStyle();
        }

        static readonly string[] TabNames = { "一般", "战斗", "探索", "门派/城镇", "物品", "其它" };

        static float TabBodyRows()
        {
            switch (_tab)
            {
                case 0: return 7f;
                case 1: return 8f;
                case 2: return 3f;
                case 3: return 4f;
                case 4: return 1f;
                default: return 1f;
            }
        }

        static void DrawWindow(Rect window, float font, float row)
        {
            float x = window.x + font * 1.2f;
            float width = window.width - font * 2.4f;
            float y = window.y + font * 2.8f;
            DrawTabs(ref y, x, width, row);
            switch (_tab)
            {
                case 0:
                    Option(ref y, x, width, row, DateLockEnabled, "锁定日期", SetDateLockEnabled);
                    Option(ref y, x, width, row, SkillExpEnabled, "无限功法经验", SetSkillExpEnabled);
                    Option(ref y, x, width, row, MaxFavorEnabled, "最高好感度", SetMaxFavorEnabled);
                    Option(ref y, x, width, row, EquipWeightEnabled, "装备负重清零", SetEquipWeightEnabled);
                    Option(ref y, x, width, row, ItemWeightEnabled, "物品负重清零", SetItemWeightEnabled);
                    Option(ref y, x, width, row, AuctionWinEnabled, "拍卖物品归我", SetAuctionWinEnabled);
                    DrawRareOption(ref y, x, width, row);
                    break;
                case 1:
                    Option(ref y, x, width, row, InfiniteHp, "无限生命（我方）", SetInfiniteHp);
                    Option(ref y, x, width, row, InfiniteMana, "无限内力（我方）", SetInfiniteMana);
                    Option(ref y, x, width, row, InfiniteStamina, "无限体力（我方）", SetInfiniteStamina);
                    Option(ref y, x, width, row, MaxMoveEnabled, "最高移动（我方）", SetMaxMoveEnabled);
                    Option(ref y, x, width, row, OneHitEnabled, "一击必杀（我方）", SetOneHitEnabled);
                    Option(ref y, x, width, row, InfiniteAction, "无限行动（我方）", SetInfiniteAction);
                    Option(ref y, x, width, row, BuffChargeEnabled, "增益无需充能（我方）", SetBuffChargeEnabled);
                    Option(ref y, x, width, row, NoSkillCdEnabled, "外功无冷却（我方）", SetNoSkillCdEnabled);
                    break;
                case 2:
                    Option(ref y, x, width, row, ExploreFogEnabled, "探索迷雾全开", SetExploreFogEnabled);
                    Option(ref y, x, width, row, ExploreTeleportEnabled, "探索单击瞬移", SetExploreTeleportEnabled);
                    Option(ref y, x, width, row, ExplorePowerLockEnabled, "探索体力锁定", SetExplorePowerLockEnabled);
                    break;
                case 3:
                    Option(ref y, x, width, row, BuildEnabled, "建筑加速（1天完成）", SetBuildEnabled);
                    Option(ref y, x, width, row, ResearchEnabled, "科研加速（1天完成）", SetResearchEnabled);
                    Option(ref y, x, width, row, TownStateEnabled, "城镇状态充满（我方）", SetTownStateEnabled);
                    Option(ref y, x, width, row, BookWeightEnabled, "门派秘籍负重清零", SetBookWeightEnabled);
                    break;
                default:
                    GUI.Label(new Rect(x, y, width, row), "暂无选项", _labelStyle);
                    y += row;
                    break;
            }
            GUI.Label(new Rect(x, y, width, font * 4.2f), TabHint(), _labelStyle);
            y += font * 4.4f;
            if (GUI.Button(new Rect(x, y, width, font * 2.2f), "关闭", _buttonStyle))
            {
                _panelOpen = false;
                _rareMenuOpen = false;
            }
        }

        static void DrawTabs(ref float y, float x, float width, float row)
        {
            int columns = 3;
            float gap = row * 0.12f;
            float tabW = (width - gap * (columns - 1)) / columns;
            float tabH = row * 0.92f;
            var prev = GUI.backgroundColor;
            for (int i = 0; i < TabNames.Length; i++)
            {
                int col = i % columns;
                int line = i / columns;
                var rect = new Rect(x + col * (tabW + gap), y + line * (tabH + gap), tabW, tabH);
                GUI.backgroundColor = _tab == i ? new Color(0.86f, 0.72f, 0.38f, 1f) : new Color(0.55f, 0.5f, 0.42f, 1f);
                if (GUI.Button(rect, TabNames[i], _buttonStyle) && _tab != i)
                {
                    _tab = i;
                    _rareMenuOpen = false;
                }
            }
            GUI.backgroundColor = prev;
            y += 2f * (tabH + gap) + gap;
        }

        static string TabHint()
        {
            switch (_tab)
            {
                case 0: return "锁定日期只锁定年月日，不锁时辰。锁定品级只影响之后获得的物品。F8 开关窗口。";
                case 1: return "带「我方」的选项只作用于我方。F8 开关窗口。";
                case 2: return "探索选项只在任务或奇遇地图内生效。F8 开关窗口。";
                case 3: return "城镇状态只作用于我方城、镇、乡、村。F8 开关窗口。";
                default: return "这一页留待以后的功能。F8 开关窗口。";
            }
        }

        static void DrawRareOption(ref float y, float x, float width, float row)
        {
            var names = GradeNames();
            int index = ClampedRare(names.Length);
            float gradeWidth = Mathf.Max(row * 3.6f, width * 0.28f);
            float gap = row * 0.2f;
            bool next = DrawOption(new Rect(x, y, width - gradeWidth - gap, row), ItemRareLockEnabled, "锁定物品品级");
            if (next != ItemRareLockEnabled)
                SetItemRareLockEnabled(next);
            var gradeRect = new Rect(x + width - gradeWidth, y + row * 0.1f, gradeWidth, row * 0.8f);
            if (GUI.Button(gradeRect, names[index] + (_rareMenuOpen ? " ▴" : " ▾"), _buttonStyle))
                _rareMenuOpen = !_rareMenuOpen;
            y += row;
            if (!_rareMenuOpen)
                return;
            for (int i = 0; i < names.Length; i++)
            {
                if (GUI.Button(new Rect(gradeRect.x, y, gradeWidth, row * 0.8f), names[i], _buttonStyle))
                {
                    SetItemRareIndex(i);
                    _rareMenuOpen = false;
                }
                y += row * 0.9f;
            }
        }

        static void Option(ref float y, float x, float width, float row, bool current, string label, Action<bool> set)
        {
            bool next = DrawOption(new Rect(x, y, width, row), current, label);
            if (next != current)
                set(next);
            y += row;
        }

        static bool DrawOption(Rect row, bool value, string label)
        {
            float box = _optionStyle.fontSize;
            float gap = box * 0.65f;
            float boxY = row.y + (row.height - box) * 0.5f;
            var boxRect = new Rect(row.x, boxY, box, box);
            if (GUI.Button(row, GUIContent.none, _hitStyle))
                value = !value;

            Texture2D tex = WhiteTex();
            Color prev = GUI.color;
            GUI.color = new Color(0.82f, 0.78f, 0.62f, 1f);
            GUI.DrawTexture(boxRect, tex);
            float border = Mathf.Max(1f, box * 0.12f);
            GUI.color = new Color(0.16f, 0.14f, 0.11f, 1f);
            GUI.DrawTexture(new Rect(boxRect.x + border, boxRect.y + border, box - border * 2f, box - border * 2f), tex);
            if (value)
            {
                GUI.color = new Color(0.93f, 0.78f, 0.28f, 1f);
                float inner = Mathf.Max(2f, box * 0.28f);
                GUI.DrawTexture(new Rect(boxRect.x + inner, boxRect.y + inner, box - inner * 2f, box - inner * 2f), tex);
            }
            GUI.color = prev;
            GUI.Label(new Rect(boxRect.xMax + gap, row.y, row.width - box - gap, row.height), label, _optionStyle);
            return value;
        }

        static Texture2D _windowTex;

        static Texture2D WindowTex()
        {
            if (_windowTex != null)
                return _windowTex;
            _windowTex = new Texture2D(1, 1);
            _windowTex.SetPixel(0, 0, new Color(0.2f, 0.16f, 0.12f, 1f));
            _windowTex.Apply();
            return _windowTex;
        }

        static void ApplyOpaque(GUIStyle style, Texture2D background, Color text)
        {
            style.normal.background = background;
            style.onNormal.background = background;
            style.hover.background = background;
            style.onHover.background = background;
            style.focused.background = background;
            style.onFocused.background = background;
            style.active.background = background;
            style.onActive.background = background;
            style.normal.textColor = text;
            style.onNormal.textColor = text;
            style.hover.textColor = text;
            style.focused.textColor = text;
            style.active.textColor = text;
        }

        static Texture2D WhiteTex()
        {
            if (_whiteTex != null)
                return _whiteTex;
            _whiteTex = new Texture2D(1, 1);
            _whiteTex.SetPixel(0, 0, Color.white);
            _whiteTex.Apply();
            return _whiteTex;
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        static void TogglePanel()
        {
            if (_hotkeyFrame == Time.frameCount)
                return;
            _hotkeyFrame = Time.frameCount;
            _panelOpen = !_panelOpen;
            if (!_panelOpen)
                _rareMenuOpen = false;
        }

        static void PollHotkey()
        {
            bool held = false;
            try
            {
                held = (GetAsyncKeyState(0x77) & 0x8000) != 0;
            }
            catch
            {
                held = false;
            }
            if (!held)
            {
                try
                {
                    held = Input.GetKey(KeyCode.F8);
                }
                catch
                {
                    held = false;
                }
            }
            if (held && !_f8Held)
                TogglePanel();
            _f8Held = held;
        }

        static void HoldGameInput()
        {
            if (!_panelOpen)
            {
                if (_inputHeld)
                    ReleaseGameInput();
                return;
            }

            try
            {
                if (!_capturedIgnore)
                {
                    _savedIgnore = UICamera.ignoreAllEvents;
                    _capturedIgnore = true;
                }
                UICamera.ignoreAllEvents = true;
                UICamera.hoveredObject = null;
            }
            catch
            {
            }
            try
            {
                MouseController.hoveredObject = null;
            }
            catch
            {
            }
            HoldOf<UICamera>();
            HoldOf<MouseController>();
            HoldCameraClicks();
            _inputHeld = true;
            if (!_loggedInputHold)
            {
                _loggedInputHold = true;
                Log($"窗口打开时已暂停 {_heldBehaviours.Count} 个游戏输入组件。");
            }
        }

        static void HoldOf<T>() where T : Behaviour
        {
            try
            {
                var found = UnityEngine.Object.FindObjectsOfType<T>();
                if (found == null)
                    return;
                foreach (var item in found)
                    HoldOne(item);
            }
            catch (Exception ex)
            {
                if (!_loggedInputHold && _instance != null)
                {
                    _loggedInputHold = true;
                    _instance.LoggerInstance.Error($"暂停游戏输入失败: {ex.Message}");
                }
            }
        }

        static void HoldOne(Behaviour behaviour)
        {
            if (behaviour == null || !behaviour.enabled || AlreadyHeld(behaviour))
                return;
            bool useMouse = true;
            bool useTouch = true;
            var camera = behaviour as UICamera;
            if (camera != null)
            {
                useMouse = camera.useMouse;
                useTouch = camera.useTouch;
                camera.useMouse = false;
                camera.useTouch = false;
            }
            behaviour.enabled = false;
            _heldBehaviours.Add(behaviour);
            _heldUseMouse.Add(useMouse);
            _heldUseTouch.Add(useTouch);
        }

        static bool AlreadyHeld(Behaviour behaviour)
        {
            for (int i = 0; i < _heldBehaviours.Count; i++)
            {
                if (_heldBehaviours[i] == behaviour)
                    return true;
            }
            return false;
        }

        static void HoldCameraClicks()
        {
            try
            {
                var found = UnityEngine.Object.FindObjectsOfType<Camera>();
                if (found == null)
                    return;
                foreach (var cam in found)
                {
                    if (cam == null || CameraHeld(cam))
                        continue;
                    _heldCameras.Add(cam);
                    _heldMasks.Add(cam.eventMask);
                    cam.eventMask = 0;
                }
            }
            catch (Exception ex)
            {
                if (!_loggedInputHold && _instance != null)
                {
                    _loggedInputHold = true;
                    _instance.LoggerInstance.Error($"暂停摄像机点击失败: {ex.Message}");
                }
            }
        }

        static bool CameraHeld(Camera camera)
        {
            for (int i = 0; i < _heldCameras.Count; i++)
            {
                if (_heldCameras[i] == camera)
                    return true;
            }
            return false;
        }

        static void ReleaseGameInput()
        {
            for (int i = 0; i < _heldBehaviours.Count; i++)
            {
                var behaviour = _heldBehaviours[i];
                if (behaviour == null)
                    continue;
                var camera = behaviour as UICamera;
                if (camera != null)
                {
                    camera.useMouse = _heldUseMouse[i];
                    camera.useTouch = _heldUseTouch[i];
                }
                behaviour.enabled = true;
            }
            _heldBehaviours.Clear();
            _heldUseMouse.Clear();
            _heldUseTouch.Clear();
            for (int i = 0; i < _heldCameras.Count; i++)
            {
                var cam = _heldCameras[i];
                if (cam != null)
                    cam.eventMask = _heldMasks[i];
            }
            _heldCameras.Clear();
            _heldMasks.Clear();
            if (_capturedIgnore)
            {
                try
                {
                    UICamera.ignoreAllEvents = _savedIgnore;
                }
                catch
                {
                }
                _capturedIgnore = false;
            }
            _inputHeld = false;
            _loggedInputHold = false;
        }

        static void SyncSavedToggles()
        {
            Pull(_infiniteHp, () => InfiniteHp, value => InfiniteHp = value);
            Pull(_infiniteMana, () => InfiniteMana, value => InfiniteMana = value);
            Pull(_infiniteStamina, () => InfiniteStamina, value => InfiniteStamina = value);
            Pull(_maxMoveEnabled, () => MaxMoveEnabled, value => MaxMoveEnabled = value);
            Pull(_oneHitEnabled, () => OneHitEnabled, value => OneHitEnabled = value);
            Pull(_infiniteAction, () => InfiniteAction, value => InfiniteAction = value);
            Pull(_buffChargeEnabled, () => BuffChargeEnabled, value => BuffChargeEnabled = value);
            Pull(_noSkillCdEnabled, () => NoSkillCdEnabled, value => NoSkillCdEnabled = value);
            Pull(_buildEnabled, () => BuildEnabled, value => BuildEnabled = value);
            Pull(_researchEnabled, () => ResearchEnabled, value => ResearchEnabled = value);
            Pull(_equipWeightEnabled, () => EquipWeightEnabled, value => EquipWeightEnabled = value);
            Pull(_itemWeightEnabled, () => ItemWeightEnabled, value => ItemWeightEnabled = value);
            Pull(_bookWeightEnabled, () => BookWeightEnabled, value => BookWeightEnabled = value);
            Pull(_dateLockEnabled, () => DateLockEnabled, value => DateLockEnabled = value);
            Pull(_skillExpEnabled, () => SkillExpEnabled, value => SkillExpEnabled = value);
            Pull(_maxFavorEnabled, () => MaxFavorEnabled, value => MaxFavorEnabled = value);
            Pull(_townStateEnabled, () => TownStateEnabled, value => TownStateEnabled = value);
            Pull(_exploreFogEnabled, () => ExploreFogEnabled, value => ExploreFogEnabled = value);
            Pull(_exploreTeleportEnabled, () => ExploreTeleportEnabled, value => ExploreTeleportEnabled = value);
            Pull(_explorePowerLockEnabled, () => ExplorePowerLockEnabled, value => ExplorePowerLockEnabled = value);
            Pull(_itemRareLockEnabled, () => ItemRareLockEnabled, value => ItemRareLockEnabled = value);
            Pull(_auctionWinEnabled, () => AuctionWinEnabled, value => AuctionWinEnabled = value);
            if (_itemRareIndex != null && ItemRareIndex != _itemRareIndex.Value)
                ItemRareIndex = _itemRareIndex.Value;
        }

        static void Pull(MelonPreferences_Entry<bool> entry, Func<bool> get, Action<bool> assign)
        {
            if (entry == null)
                return;
            bool value = entry.Value;
            if (get() != value)
                assign(value);
        }

        static void ApplySavedWhenReady()
        {
            if (_appliedSaved || CurrentPlayer() == null)
                return;
            _appliedSaved = true;
            bool any = BuildEnabled || ResearchEnabled || DateLockEnabled
                || EquipWeightEnabled || ItemWeightEnabled || BookWeightEnabled
                || TownStateEnabled;
            if (!any)
                return;
            if (BuildEnabled)
                ApplyBuild();
            if (ResearchEnabled)
                ApplyResearch();
            if (EquipWeightEnabled || ItemWeightEnabled || BookWeightEnabled)
                ApplyWeight();
            if (DateLockEnabled)
            {
                _dateCaptured = false;
                RestoreDate();
            }
            if (TownStateEnabled)
                ApplyTownState();
            Log("已按上次保存的开关生效。");
        }

        public static void SetBuildEnabled(bool value)
        {
            BuildEnabled = value;
            SaveEntry(_buildEnabled, value);
            Log(value ? "建筑加速已打开" : "建筑加速已关闭");
            if (!value)
                return;
            int changed = ApplyBuild();
            Log($"当前把 {changed} 个建筑计时压到 1 天。");
        }

        public static void SetResearchEnabled(bool value)
        {
            ResearchEnabled = value;
            SaveEntry(_researchEnabled, value);
            Log(value ? "科研加速已打开" : "科研加速已关闭");
            if (!value)
                return;
            int changed = ApplyResearch();
            Log(changed > 0 ? "当前门派科研已压到剩余 1 天。" : "当前没有进行中的门派科研。");
        }

        public static void SetInfiniteHp(bool value)
        {
            InfiniteHp = value;
            SaveToggle(_infiniteHp, value, "无限生命");
        }

        public static void SetInfiniteMana(bool value)
        {
            InfiniteMana = value;
            SaveToggle(_infiniteMana, value, "无限内力");
        }

        public static void SetInfiniteStamina(bool value)
        {
            InfiniteStamina = value;
            SaveToggle(_infiniteStamina, value, "无限体力");
        }

        public static void SetMaxMoveEnabled(bool value)
        {
            MaxMoveEnabled = value;
            SaveToggle(_maxMoveEnabled, value, "最高移动");
        }

        public static void SetOneHitEnabled(bool value)
        {
            OneHitEnabled = value;
            SaveToggle(_oneHitEnabled, value, "一击必杀");
        }

        public static void SetInfiniteAction(bool value)
        {
            InfiniteAction = value;
            SaveToggle(_infiniteAction, value, "无限行动");
        }

        public static void SetBuffChargeEnabled(bool value)
        {
            BuffChargeEnabled = value;
            SaveToggle(_buffChargeEnabled, value, "增益无需充能");
        }

        public static void SetNoSkillCdEnabled(bool value)
        {
            NoSkillCdEnabled = value;
            SaveToggle(_noSkillCdEnabled, value, "外功无冷却");
        }

        static void SaveToggle(MelonPreferences_Entry<bool> entry, bool value, string name)
        {
            SaveEntry(entry, value);
            Log(value ? name + "已打开" : name + "已关闭");
        }

        public static void SetEquipWeightEnabled(bool value)
        {
            EquipWeightEnabled = value;
            SaveEntry(_equipWeightEnabled, value);
            Log(value ? "装备负重清零已打开" : "装备负重清零已关闭");
            if (value)
                ApplyWeight();
        }

        public static void SetItemWeightEnabled(bool value)
        {
            ItemWeightEnabled = value;
            SaveEntry(_itemWeightEnabled, value);
            Log(value ? "物品负重清零已打开" : "物品负重清零已关闭");
            if (value)
                ApplyWeight();
        }

        public static void SetBookWeightEnabled(bool value)
        {
            BookWeightEnabled = value;
            SaveEntry(_bookWeightEnabled, value);
            Log(value ? "门派秘籍负重清零已打开" : "门派秘籍负重清零已关闭");
            if (value)
                ApplyWeight();
        }

        public static void SetDateLockEnabled(bool value)
        {
            DateLockEnabled = value;
            SaveEntry(_dateLockEnabled, value);
            _dateCaptured = false;
            if (value)
                RestoreDate();
            Log(value ? "日期已锁定" : "日期锁定已关闭");
        }

        public static void SetSkillExpEnabled(bool value)
        {
            SkillExpEnabled = value;
            SaveEntry(_skillExpEnabled, value);
            Log(value ? "无限功法经验已打开" : "无限功法经验已关闭");
        }

        public static void SetMaxFavorEnabled(bool value)
        {
            MaxFavorEnabled = value;
            SaveEntry(_maxFavorEnabled, value);
            Log(value ? "最高好感度已打开" : "最高好感度已关闭");
        }

        public static void SetTownStateEnabled(bool value)
        {
            TownStateEnabled = value;
            SaveEntry(_townStateEnabled, value);
            Log(value ? "城镇状态充满已打开" : "城镇状态充满已关闭，当前数值保留");
            if (value)
                ApplyTownState();
        }

        public static void SetExploreFogEnabled(bool value)
        {
            ExploreFogEnabled = value;
            SaveEntry(_exploreFogEnabled, value);
            Log(value ? "探索迷雾全开已打开" : "探索迷雾全开已关闭");
            if (value)
                ApplyExplore();
        }

        public static void SetExploreTeleportEnabled(bool value)
        {
            ExploreTeleportEnabled = value;
            SaveEntry(_exploreTeleportEnabled, value);
            Log(value ? "探索单击瞬移已打开" : "探索单击瞬移已关闭");
        }

        public static void SetExplorePowerLockEnabled(bool value)
        {
            ExplorePowerLockEnabled = value;
            SaveEntry(_explorePowerLockEnabled, value);
            Log(value ? "探索体力锁定已打开" : "探索体力锁定已关闭，当前体力保留");
            if (value)
                ApplyExplore();
        }

        public static void SetAuctionWinEnabled(bool value)
        {
            AuctionWinEnabled = value;
            SaveEntry(_auctionWinEnabled, value);
            Log(value ? "拍卖物品归我已打开" : "拍卖物品归我已关闭");
        }

        public static void SetItemRareLockEnabled(bool value)
        {
            ItemRareLockEnabled = value;
            SaveEntry(_itemRareLockEnabled, value);
            var names = GradeNames();
            string grade = names[ClampedRare(names.Length)];
            Log(value ? "锁定物品品级已打开：" + grade : "锁定物品品级已关闭");
        }

        public static void SetItemRareIndex(int value)
        {
            var names = GradeNames();
            if (names.Length > 0)
                value = Mathf.Clamp(value, 0, names.Length - 1);
            ItemRareIndex = value;
            if (_itemRareIndex != null && _itemRareIndex.Value != value)
            {
                _itemRareIndex.Value = value;
                MelonPreferences.Save();
            }
            Log("物品品级已选为" + names[ClampedRare(names.Length)]);
        }

        static readonly string[] FallbackGrades = { "品级1", "品级2", "品级3", "品级4", "品级5", "品级6" };
        static string[] _gradeNames;
        static int _gradeNamesFrame = -1;
        static bool _loggedGrades;
        static int _grantingRare;

        static string[] GradeNames()
        {
            int frame = 0;
            try
            {
                frame = Time.frameCount;
            }
            catch
            {
                frame = 0;
            }
            if (_gradeNames != null && _gradeNamesFrame == frame)
                return _gradeNames;

            string[] names = null;
            try
            {
                var treasure = CopyRareNames(GlobalData.TreasureRareLvName);
                var book = CopyRareNames(GlobalData.BookRareLvName);
                if (HasGradeWord(treasure))
                    names = treasure;
                else if (HasGradeWord(book))
                    names = book;
                else if (treasure != null && treasure.Length > 0)
                    names = treasure;
                else
                    names = book;
            }
            catch
            {
                names = null;
            }
            if (names == null || names.Length == 0)
                names = FallbackGrades;
            else if (!_loggedGrades)
            {
                _loggedGrades = true;
                Log("物品品级：" + string.Join("、", names));
            }
            _gradeNames = names;
            _gradeNamesFrame = frame;
            return names;
        }

        static string[] CopyRareNames(Il2CppSystem.Collections.Generic.List<string> source)
        {
            if (source == null || source.Count <= 0)
                return null;
            var names = new string[source.Count];
            for (int i = 0; i < source.Count; i++)
            {
                var name = source[i];
                names[i] = string.IsNullOrEmpty(name) ? ("品级" + (i + 1)) : name;
            }
            return names;
        }

        static bool HasGradeWord(string[] names)
        {
            if (names == null)
                return false;
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                if (name != null && (name.Contains("残品") || name.Contains("下品") || name.Contains("极品") || name.Contains("残本")))
                    return true;
            }
            return false;
        }

        static int ClampedRare(int count)
        {
            if (count <= 0)
                return 0;
            int index = ItemRareIndex;
            if (index < 0 || index >= count)
                return count - 1;
            return index;
        }

        static void ApplyRare(ItemData item)
        {
            if (!ItemRareLockEnabled || item == null)
                return;
            int grade = ClampedRare(GradeNames().Length);
            if (item.rareLv != grade)
                item.rareLv = grade;
        }

        static void ApplyRareList(ItemListData list)
        {
            if (list == null)
                return;
            var items = list.allItem;
            if (items == null)
                return;
            for (int i = 0; i < items.Count; i++)
                ApplyRare(items[i]);
        }

        public static void ForceRolledRare(ref int __result)
        {
            if (!ItemRareLockEnabled)
                return;
            __result = ClampedRare(GradeNames().Length);
        }

        public static bool BeforeGotItem(HeroData __instance, ItemData itemData, ref bool __state)
        {
            if (TryRedirectAuction(__instance, itemData))
            {
                __state = false;
                return false;
            }
            if (IsTrainee(__instance))
                NoteAuctionLot(itemData);
            BeginGrant(__instance != null && IsTrainee(__instance), itemData, ref __state);
            return true;
        }

        public static void AfterGotItem(ItemData itemData, bool __state)
        {
            if (__state)
                ApplyRare(itemData);
        }

        public static bool BeforeGotBundle(HeroData __instance, ItemListData _itemListData, ref bool __state)
        {
            PullAuctionLots(__instance, _itemListData);
            __state = false;
            if (!ItemRareLockEnabled || __instance == null || !IsTrainee(__instance))
                return true;
            __state = true;
            _grantingRare++;
            ApplyRareList(_itemListData);
            return true;
        }

        public static void AfterGotBundle(ItemListData _itemListData, bool __state)
        {
            if (__state)
                ApplyRareList(_itemListData);
        }

        public static bool BeforeBagItem(ItemListData __instance, ItemData targetItem, ref bool __state)
        {
            if (!IsPlayerBag(__instance) && TryRedirectAuction(null, targetItem))
            {
                __state = false;
                return false;
            }
            if (IsPlayerBag(__instance))
                NoteAuctionLot(targetItem);
            BeginGrant(IsPlayerBag(__instance), targetItem, ref __state);
            return true;
        }

        public static void AfterBagItem(ItemData targetItem, bool __state)
        {
            if (__state)
                ApplyRare(targetItem);
        }

        public static bool BeforeBagBundle(ItemListData __instance, ItemListData target, ref bool __state)
        {
            if (!IsPlayerBag(__instance))
                PullAuctionLots(null, target);
            __state = false;
            if (!ItemRareLockEnabled || !IsPlayerBag(__instance))
                return true;
            __state = true;
            _grantingRare++;
            ApplyRareList(target);
            return true;
        }

        public static void AfterBagBundle(ItemListData target, bool __state)
        {
            if (__state)
                ApplyRareList(target);
        }

        public static void KeepGrantedRare(ItemData __instance)
        {
            if (_grantingRare <= 0)
                return;
            ApplyRare(__instance);
        }

        public static Exception EndGrant(bool __state, Exception __exception)
        {
            if (__state && _grantingRare > 0)
                _grantingRare--;
            return __exception;
        }

        static ItemData _auctionDealLot;
        static bool _auctionDealClaimed;
        static bool _givingAuctionItem;

        static bool _auctionClaim;

        static void PatchAuctionDeal(HarmonyLib.Harmony harmony)
        {
            var nested = typeof(AuctionController).GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < nested.Length; i++)
            {
                if (nested[i].Name.IndexOf("StartAuctionDeal", StringComparison.Ordinal) < 0)
                    continue;
                Patch(harmony, AccessTools.Method(nested[i], "MoveNext"), nameof(BeforeAuctionDealStep), nameof(AfterAuctionDealStep));
            }
        }

        public static void ClaimAuctionWinner(AuctionController __instance)
        {
            if (!AuctionWinEnabled || __instance == null)
                return;
            _auctionClaim = true;
            if (!BuyingLot(__instance))
                return;
            var player = CurrentPlayer();
            if (player == null)
                return;
            _auctionDealLot = CurrentLot(__instance);
            _auctionDealClaimed = BagHas(player, _auctionDealLot);
            __instance.nowOfferHero = player;
            __instance.nextOfferHero = player;
            float pay = __instance.playerOfferMoney;
            if (pay < 0f)
                pay = 0f;
            __instance.nowOfferMoney = pay;
        }

        public static void BeforeAuctionDealStep(object __instance)
        {
            if (!AuctionWinEnabled || __instance == null || DealState(__instance) != 0)
                return;
            ClaimAuctionWinner(DealAuction(__instance));
        }

        public static void AfterAuctionDealStep(object __instance, ref bool __result)
        {
            if (!AuctionWinEnabled || __result)
                return;
            var auction = DealAuction(__instance);
            if (auction == null || !BuyingLot(auction))
            {
                _auctionClaim = false;
                return;
            }
            if (_auctionDealClaimed || BagHas(CurrentPlayer(), _auctionDealLot))
            {
                _auctionClaim = false;
                return;
            }
            GiveLot(_auctionDealLot);
            _auctionClaim = false;
        }

        public static void GiveLeftoverLots(AuctionController __instance)
        {
            if (!AuctionWinEnabled || __instance == null)
            {
                _auctionClaim = false;
                return;
            }
            var list = __instance.auctionItemList;
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (IsPlayerSale(__instance, item) || BagHas(CurrentPlayer(), item))
                        continue;
                    GiveLot(item);
                }
            }
            _auctionClaim = false;
        }

        static AuctionController DealAuction(object step)
        {
            try
            {
                var prop = step.GetType().GetProperty("__4__this", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                return prop == null ? null : prop.GetValue(step) as AuctionController;
            }
            catch
            {
                return null;
            }
        }

        static int DealState(object step)
        {
            try
            {
                var prop = step.GetType().GetProperty("__1__state", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                return prop == null ? -1 : Convert.ToInt32(prop.GetValue(step));
            }
            catch
            {
                return -1;
            }
        }

        static AuctionController AuctionNow()
        {
            try
            {
                return AuctionController.Instance;
            }
            catch
            {
                return null;
            }
        }

        static bool AuctionBusy(AuctionController auction)
        {
            if (auction == null)
                return false;
            var step = auction.auctionStep;
            return step == AuctionStep.Offer || step == AuctionStep.Wait || step == AuctionStep.Deal || step == AuctionStep.End;
        }

        static ItemData CurrentLot(AuctionController auction)
        {
            var list = auction == null ? null : auction.auctionItemList;
            if (list == null || list.Count == 0)
                return null;
            int index = auction.round;
            if (index < 0 || index >= list.Count)
                index = Mathf.Clamp(index, 0, list.Count - 1);
            return list[index];
        }

        static bool BuyingLot(AuctionController auction)
        {
            var lot = CurrentLot(auction);
            return lot != null && !IsPlayerSale(auction, lot);
        }

        static bool IsPlayerSale(AuctionController auction, ItemData item)
        {
            return item != null && auction != null && auction.playerSellItem == item;
        }

        static bool IsListedLot(AuctionController auction, ItemData item)
        {
            if (auction == null || item == null)
                return false;
            var list = auction.auctionItemList;
            if (list == null)
                return false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == item)
                    return true;
            }
            return false;
        }

        static bool BagHas(HeroData hero, ItemData item)
        {
            if (hero == null || item == null || hero.itemListData == null)
                return false;
            var items = hero.itemListData.allItem;
            if (items == null)
                return false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == item)
                    return true;
            }
            return false;
        }

        static void NoteAuctionLot(ItemData item)
        {
            if (!AuctionWinEnabled || item == null)
                return;
            var auction = AuctionNow();
            if (!AuctionBusy(auction) || !IsListedLot(auction, item))
                return;
            if (!_auctionClaim && auction.auctionStep != AuctionStep.Deal && auction.auctionStep != AuctionStep.End)
                return;
            _auctionDealClaimed = true;
        }

        static bool TryRedirectAuction(HeroData hero, ItemData item)
        {
            if (!AuctionWinEnabled || _givingAuctionItem || item == null)
                return false;
            if (hero != null && IsTrainee(hero))
                return false;
            var auction = AuctionNow();
            if (!AuctionBusy(auction) || !IsListedLot(auction, item) || IsPlayerSale(auction, item))
                return false;
            if (!_auctionClaim && auction.auctionStep != AuctionStep.Deal && auction.auctionStep != AuctionStep.End)
                return false;
            GiveLot(item);
            return true;
        }

        static void PullAuctionLots(HeroData hero, ItemListData incoming)
        {
            if (!AuctionWinEnabled || _givingAuctionItem || incoming == null)
                return;
            if (hero != null && IsTrainee(hero))
                return;
            var auction = AuctionNow();
            if (!AuctionBusy(auction))
                return;
            if (!_auctionClaim && auction.auctionStep != AuctionStep.Deal && auction.auctionStep != AuctionStep.End)
                return;
            var items = incoming.allItem;
            if (items == null)
                return;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                if (!IsListedLot(auction, item) || IsPlayerSale(auction, item))
                    continue;
                items.RemoveAt(i);
                GiveLot(item);
            }
        }

        static void GiveLot(ItemData item)
        {
            if (item == null || _givingAuctionItem)
                return;
            var player = CurrentPlayer();
            if (player == null)
                return;
            if (BagHas(player, item))
            {
                _auctionDealClaimed = true;
                return;
            }
            _givingAuctionItem = true;
            _auctionDealClaimed = true;
            try
            {
                player.GetItem(item, true);
            }
            catch (Exception ex)
            {
                Log("拍卖物品交给主角失败。" + ex.Message);
            }
            finally
            {
                _givingAuctionItem = false;
            }
        }

        static void BeginGrant(bool receive, ItemData item, ref bool __state)
        {
            __state = false;
            if (!ItemRareLockEnabled || !receive)
                return;
            __state = true;
            _grantingRare++;
            ApplyRare(item);
        }

        static void SaveEntry(MelonPreferences_Entry<bool> entry, bool value)
        {
            if (entry == null || entry.Value == value)
                return;
            entry.Value = value;
            MelonPreferences.Save();
        }

        static void Log(string message)
        {
            if (_instance != null)
                _instance.LoggerInstance.Msg(message);
        }

        public static bool BeforeDayPasses()
        {
            if (DateLockEnabled)
                return false;
            if (BuildEnabled)
                ApplyBuild();
            if (ResearchEnabled)
                ApplyResearch();
            return true;
        }

        public static bool BeforeTimedDayPass(ref int deltaDay)
        {
            if (DateLockEnabled && deltaDay > 0)
                return false;
            if (BuildEnabled && _houseUpgradeDepth > 0 && deltaDay > TargetDays)
                deltaDay = TargetDays;
            if (BuildEnabled)
                ApplyBuild();
            if (ResearchEnabled)
                ApplyResearch();
            return true;
        }

        public static void RestoreLockedDate()
        {
            if (DateLockEnabled)
                RestoreDate();
        }

        static WorldData CurrentWorld()
        {
            try
            {
                var data = GameDataController.Instance;
                return data != null && data.gameSaveData != null ? data.gameSaveData.WorldData : null;
            }
            catch
            {
                return null;
            }
        }

        static void RestoreDate()
        {
            if (!DateLockEnabled)
                return;
            try
            {
                var time = CurrentWorld()?.worldTime;
                if (time == null)
                    return;
                if (!_dateCaptured)
                {
                    _lockYear = time.year;
                    _lockMonth = time.month;
                    _lockDay = time.day;
                    _dateCaptured = true;
                    return;
                }
                if (time.year != _lockYear)
                    time.year = _lockYear;
                if (time.month != _lockMonth)
                    time.month = _lockMonth;
                if (time.day != _lockDay)
                    time.day = _lockDay;
            }
            catch (Exception ex)
            {
                if (_instance != null)
                    _instance.LoggerInstance.Error($"锁定日期失败: {ex.Message}");
            }
        }

        public static void EnterHouseUpgrade()
        {
            if (BuildEnabled)
                _houseUpgradeDepth++;
        }

        public static void LeaveHouseUpgrade()
        {
            if (_houseUpgradeDepth > 0)
                _houseUpgradeDepth--;
        }

        public static void AfterConstructionStarted()
        {
            if (BuildEnabled)
                ApplyBuild();
        }

        public static void CapResearchLeftDay(ForceTechLvData __instance, ref int __result)
        {
            if (_readingRealResearchDays || !ResearchEnabled || __result <= TargetDays || !IsPlayerCurrentResearch(__instance))
                return;
            __result = TargetDays;
        }

        public static void MaxAllyMoveRange(HeroData __instance, ref int __result)
        {
            if (!MaxMoveEnabled || __result >= MaxMoveRange || !IsAllyHero(__instance))
                return;
            __result = MaxMoveRange;
        }

        public static void BlockAllyHp(BattleUnit __instance, ref float num)
        {
            if (!InfiniteHp || num >= 0f || !IsAllyUnit(__instance))
                return;
            num = 0f;
        }

        public static void BlockAllyMana(BattleUnit __instance, ref float num)
        {
            if (!InfiniteMana || num >= 0f || !IsAllyUnit(__instance))
                return;
            num = 0f;
        }

        public static void BlockAllyStamina(BattleUnit __instance, ref float num)
        {
            if (!InfiniteStamina || num >= 0f || !IsAllyUnit(__instance))
                return;
            num = 0f;
        }

        public static void BeforeOneHit(BattleUnit source, BattleUnit target, bool noDead)
        {
            if (!OneHitEnabled || noDead || target == null || target.heroData == null)
                return;
            if (!IsAllyUnit(source) || IsAllyUnit(target))
                return;
            if (target.heroData.hp > 0.01f)
                target.heroData.hp = 0.01f;
        }

        public static void AfterOneHitDamage(BattleUnit attackUnit, BattleUnit targetUnit, ref float __result)
        {
            if (!OneHitEnabled || !IsAllyUnit(attackUnit) || IsAllyUnit(targetUnit))
                return;
            float hp = targetUnit != null && targetUnit.heroData != null ? targetUnit.heroData.hp : 0f;
            float kill = hp + 99999f;
            if (__result < kill)
                __result = kill;
        }

        public static void ZeroEquipWeight(HeroEquipmentData __instance, ref float __result)
        {
            if (EquipWeightEnabled && IsPlayerEquipment(__instance))
                __result = 0f;
        }

        public static void AfterRecountEquip(HeroEquipmentData __instance)
        {
            if (EquipWeightEnabled && IsPlayerEquipment(__instance))
                __instance.equipmentWeight = 0f;
        }

        public static void ZeroEquipLevel(HeroData __instance, ref int __result)
        {
            if (EquipWeightEnabled && IsPlayerHero(__instance))
                __result = 0;
        }

        public static void ZeroTrackedWeight(ItemListData __instance, ref float value)
        {
            if (value <= 0f)
                return;
            if (ItemWeightEnabled && IsPlayerBag(__instance))
                value = 0f;
            else if (BookWeightEnabled && IsPlayerBookStorage(__instance))
                value = 0f;
        }

        public static bool NotOverweight(object[] __args, ref bool __result)
        {
            if (!ItemWeightEnabled || __args == null || __args.Length == 0 || !(__args[0] is HeroData hero) || !IsPlayerHero(hero))
                return true;
            __result = false;
            return false;
        }

        public static bool ForceOwnedDuration(object __instance, object[] __args, ref int __result)
        {
            if (!BuildEnabled)
                return true;

            if (__instance is AreaBuildingData building && IsPrivateHouse(building))
            {
                __result = TargetDays;
                return false;
            }

            if (__instance is AreaBuildingDataBase template && IsPrivateHouseName(template.name))
            {
                __result = TargetDays;
                return false;
            }

            var area = ResolveArea(__instance, __args);
            if (area == null || !Owns(area))
                return true;

            __result = TargetDays;
            return false;
        }

        static void PatchGetter(HarmonyLib.Harmony harmony, MethodBase method)
        {
            Patch(harmony, method, nameof(ForceOwnedDuration), null);
        }

        static void Patch(HarmonyLib.Harmony harmony, MethodBase method, string prefix, string postfix, string finalizer = null)
        {
            if (method == null)
            {
                if (_instance != null)
                    _instance.LoggerInstance.Warning("有一个目标方法没找到，对应的那一条加速不会生效。");
                return;
            }

            try
            {
                harmony.Patch(
                    method,
                    prefix: prefix == null ? null : new HarmonyMethod(typeof(TrainerMod), prefix),
                    postfix: postfix == null ? null : new HarmonyMethod(typeof(TrainerMod), postfix),
                    finalizer: finalizer == null ? null : new HarmonyMethod(typeof(TrainerMod), finalizer));
            }
            catch (Exception ex)
            {
                if (_instance != null)
                    _instance.LoggerInstance.Warning($"挂钩失败。{method.DeclaringType?.Name}.{method.Name}: {ex.Message}");
            }
        }

        static int ApplyBuild()
        {
            try
            {
                RefreshPlayerForce();
                var data = GameDataController.Instance;
                var world = data != null && data.gameSaveData != null ? data.gameSaveData.WorldData : null;
                if (world == null || world.Areas == null)
                    return 0;

                int changed = 0;
                var areas = world.Areas;
                for (int i = 0; i < areas.Count; i++)
                {
                    var area = areas[i];
                    if (area == null)
                        continue;
                    changed += ClampArea(area, Owns(area));
                }

                if (changed > 0)
                    TryRefreshBuildingUi();
                return changed;
            }
            catch (Exception ex)
            {
                if (_instance != null)
                    _instance.LoggerInstance.Error($"缩短工期失败: {ex.Message}");
                return 0;
            }
        }

        static void ApplyWeight()
        {
            try
            {
                RefreshPlayerForce();
                var player = CurrentPlayer();
                if (player == null)
                    return;
                if (EquipWeightEnabled && player.nowEquipment != null && player.nowEquipment.equipmentWeight != 0f)
                    player.nowEquipment.equipmentWeight = 0f;
                if (ItemWeightEnabled && player.itemListData != null && player.itemListData.weight != 0f)
                    player.itemListData.weight = 0f;
                if (BookWeightEnabled)
                {
                    var force = PlayerForce();
                    if (force != null && force.bookStorage != null && force.bookStorage.weight != 0f)
                        force.bookStorage.weight = 0f;
                }
                _loggedWeightFailure = false;
            }
            catch (Exception ex)
            {
                if (_instance != null && !_loggedWeightFailure)
                {
                    _loggedWeightFailure = true;
                    _instance.LoggerInstance.Error($"清负重失败: {ex.Message}");
                }
            }
        }

        static void ApplyExplore()
        {
            ExploreController explore;
            try
            {
                explore = ExploreController.Instance;
            }
            catch
            {
                return;
            }
            if (explore == null)
                return;
            try
            {
                if (!explore.IsExploring())
                    return;
            }
            catch
            {
                return;
            }
            if (ExploreFogEnabled && ExploreMapHasFog(explore))
            {
                try
                {
                    explore.SeeAllTile();
                    _loggedExploreFailure = false;
                }
                catch (Exception ex)
                {
                    if (_instance != null && !_loggedExploreFailure)
                    {
                        _loggedExploreFailure = true;
                        _instance.LoggerInstance.Error($"探索迷雾全开失败: {ex.Message}");
                    }
                }
            }
            if (ExplorePowerLockEnabled)
                KeepExplorePower(explore);
        }

        static bool ExploreMapHasFog(ExploreController explore)
        {
            var panel = explore.explorePanelData;
            if (panel == null || panel.exploreTiles == null)
                return false;
            var tiles = panel.exploreTiles;
            for (int i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                if (tile != null && !tile.seen)
                    return true;
            }
            return false;
        }

        public static void RevealExploreMap(ExploreController __instance)
        {
            if (!ExploreFogEnabled || __instance == null)
                return;
            try
            {
                __instance.SeeAllTile();
            }
            catch (Exception ex)
            {
                if (_instance != null && !_loggedExploreFailure)
                {
                    _loggedExploreFailure = true;
                    _instance.LoggerInstance.Error($"探索迷雾全开失败: {ex.Message}");
                }
            }
        }

        public static bool TeleportExploreClick(ExploreTileUnitController __instance)
        {
            if (!ExploreTeleportEnabled || __instance == null)
                return true;
            return !TeleportToExploreTile(__instance.exploreTileData);
        }

        public static bool TeleportExploreTile(ExploreTileData exploreTileData)
        {
            if (!ExploreTeleportEnabled)
                return true;
            return !TeleportToExploreTile(exploreTileData);
        }

        static bool TeleportToExploreTile(ExploreTileData tile)
        {
            if (tile == null || _teleportDepth > 0)
                return false;
            if (_exploreClickFrame == Time.frameCount)
                return true;
            ExploreController explore;
            try
            {
                explore = ExploreController.Instance;
            }
            catch
            {
                return false;
            }
            if (explore == null)
                return false;
            try
            {
                if (!explore.IsExploring())
                    return false;
                _exploreClickFrame = Time.frameCount;
                _teleportDepth++;
                try
                {
                    explore.PlayerEnterGrid(tile, true);
                }
                finally
                {
                    _teleportDepth--;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (_instance != null && !_loggedExploreFailure)
                {
                    _loggedExploreFailure = true;
                    _instance.LoggerInstance.Error($"探索瞬移失败: {ex.Message}");
                }
                return false;
            }
        }

        public static bool BlockExplorePowerDrain(ref int num)
        {
            if (!ExplorePowerLockEnabled || num >= 0)
                return true;
            return false;
        }

        public static void KeepExplorePower(ExploreController __instance)
        {
            if (!ExplorePowerLockEnabled || __instance == null)
                return;
            var panel = __instance.explorePanelData;
            if (panel == null || panel.maxPower <= 0 || __instance.leftPower == panel.maxPower)
                return;
            __instance.leftPower = panel.maxPower;
        }

        static void ApplyTownState()
        {
            try
            {
                RefreshPlayerForce();
                var world = CurrentWorld();
                if (world == null || world.Areas == null)
                    return;
                var areas = world.Areas;
                for (int i = 0; i < areas.Count; i++)
                {
                    var area = areas[i];
                    if (IsPlayerTown(world, area))
                        FillTown(area);
                }
                _loggedTownFailure = false;
            }
            catch (Exception ex)
            {
                if (_instance != null && !_loggedTownFailure)
                {
                    _loggedTownFailure = true;
                    _instance.LoggerInstance.Error($"城镇状态充满失败: {ex.Message}");
                }
            }
        }

        public static bool LockTownDelta(AreaData __instance, ref float num)
        {
            if (!ShouldLockTown(__instance))
                return true;
            FillTown(__instance);
            num = 0f;
            return false;
        }

        public static bool LockTownAreaState(AreaData __instance, ref float result)
        {
            if (!ShouldLockTown(__instance))
                return true;
            FillTown(__instance);
            result = 0f;
            return false;
        }

        public static bool LockTownDefenceDrop(AreaData __instance)
        {
            if (!ShouldLockTown(__instance))
                return true;
            FillTown(__instance);
            return false;
        }

        static bool ShouldLockTown(AreaData area)
        {
            if (!TownStateEnabled || area == null)
                return false;
            RefreshPlayerForce();
            return IsPlayerTown(CurrentWorld(), area);
        }

        static bool IsPlayerTown(WorldData world, AreaData area)
        {
            if (world == null || area == null || !Owns(area))
                return false;
            int id = area.areaID;
            if (world.cityAreaID != null && world.cityAreaID.Contains(id))
                return true;
            return world.villageAreaID != null && world.villageAreaID.Contains(id);
        }

        static void FillTown(AreaData area)
        {
            bool changed = false;
            if (area.maxPeople > 0f && area.people < area.maxPeople)
            {
                area.people = area.maxPeople;
                changed = true;
            }
            changed |= FillGauge(area, AreaStateType.Safe, area.safe, value => area.safe = value);
            changed |= FillGauge(area, AreaStateType.Defence, area.defence, value => area.defence = value);
            changed |= FillGauge(area, AreaStateType.Support, area.support, value => area.support = value);
            if (!changed)
                return;
            area.areaInfoDirty = true;
            area.areaDetailDirty = true;
        }

        static bool FillGauge(AreaData area, AreaStateType kind, float current, Action<float> set)
        {
            float cap = 0f;
            try
            {
                cap = area.GetMaxAreaState((int)kind);
            }
            catch
            {
                cap = 0f;
            }
            if (cap <= 0f)
                cap = 100f;
            if (current >= cap)
                return false;
            set(cap);
            return true;
        }

        static HeroData CurrentPlayer()
        {
            try
            {
                var data = GameDataController.Instance;
                var world = data != null && data.gameSaveData != null ? data.gameSaveData.WorldData : null;
                return world != null ? world.Player() : null;
            }
            catch
            {
                return null;
            }
        }

        static bool IsPlayerHero(HeroData hero)
        {
            if (hero == null)
                return false;
            var player = CurrentPlayer();
            return player != null && (player == hero || (_playerHeroId > 0 && hero.heroID == _playerHeroId));
        }

        public struct ExpSnap
        {
            public KungfuSkillLvData skill;
            public float fight;
            public float book;
        }

        static bool IsTrainee(HeroData hero)
        {
            if (IsPlayerHero(hero))
                return true;
            var player = CurrentPlayer();
            if (player == null || hero == null)
                return false;
            if (hero.inTeam && player.heroID > 0 && hero.teamLeader == player.heroID)
                return true;
            var mates = player.teamMates;
            if (mates == null)
                return false;
            int id = hero.heroID;
            for (int i = 0; i < mates.Count; i++)
            {
                if (mates[i] == id)
                    return true;
            }
            return false;
        }

        static bool IsTraineeSkill(KungfuSkillLvData skill)
        {
            if (skill == null)
                return false;
            try
            {
                var hero = skill.GetBelongHero();
                if (hero != null)
                    return IsTrainee(hero);
            }
            catch
            {
                return false;
            }
            var player = CurrentPlayer();
            if (player == null)
                return false;
            if (player.heroID > 0 && skill.belongHeroID == player.heroID)
                return true;
            var mates = player.teamMates;
            if (mates == null)
                return false;
            for (int i = 0; i < mates.Count; i++)
            {
                if (mates[i] == skill.belongHeroID)
                    return true;
            }
            return false;
        }

        static void FillToBreakthrough(KungfuSkillLvData skill)
        {
            if (skill == null || _fillingExp)
                return;
            try
            {
                if (skill.FightExpFull() && skill.BookExpFull())
                    return;
            }
            catch
            {
                return;
            }

            _fillingExp = true;
            try
            {
                skill.FullFillExp();
            }
            catch (Exception ex)
            {
                if (_instance != null && !_loggedExpFailure)
                {
                    _loggedExpFailure = true;
                    _instance.LoggerInstance.Error($"功法经验补满失败: {ex.Message}");
                }
            }
            finally
            {
                _fillingExp = false;
            }
        }

        public static void EnterExpGain(HeroData __instance, float num, KungfuSkillLvData targetSkill, ref bool __state)
        {
            __state = SkillExpEnabled && num > 0f && targetSkill != null && IsTrainee(__instance);
            if (__state)
                _expGainDepth++;
        }

        public static void AfterSkillExp(HeroData __instance, float num, KungfuSkillLvData targetSkill, bool __state)
        {
            try
            {
                if (__state)
                    FillToBreakthrough(targetSkill);
            }
            finally
            {
                if (__state && _expGainDepth > 0)
                    _expGainDepth--;
            }
        }

        public static void EnterAutoExp(HeroData __instance, ref ExpSnap[] __state)
        {
            __state = null;
            if (!SkillExpEnabled || !IsTrainee(__instance))
                return;
            var list = __instance.kungfuSkills;
            if (list == null)
                __state = Array.Empty<ExpSnap>();
            else
            {
                var snaps = new ExpSnap[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    var skill = list[i];
                    snaps[i] = new ExpSnap
                    {
                        skill = skill,
                        fight = skill != null ? skill.fightExp : 0f,
                        book = skill != null ? skill.bookExp : 0f
                    };
                }
                __state = snaps;
            }
            _expGainDepth++;
        }

        public static void LeaveAutoExp(ExpSnap[] __state)
        {
            try
            {
                if (__state == null)
                    return;
                for (int i = 0; i < __state.Length; i++)
                {
                    var snap = __state[i];
                    var skill = snap.skill;
                    if (skill == null)
                        continue;
                    if (skill.fightExp > snap.fight + 0.0001f || skill.bookExp > snap.book + 0.0001f)
                        FillToBreakthrough(skill);
                }
            }
            finally
            {
                if (__state != null && _expGainDepth > 0)
                    _expGainDepth--;
            }
        }

        public static void RaiseFightExp(KungfuSkillLvData __instance, ref float value)
        {
            if (!SkillExpEnabled || _fillingExp || __instance == null)
                return;
            float old = __instance.fightExp;
            if (value <= old + 0.0001f)
                return;
            if (old <= 0f && _expGainDepth <= 0)
                return;
            if (!IsTraineeSkill(__instance))
                return;
            FillToBreakthrough(__instance);
            if (__instance.fightExp > value)
                value = __instance.fightExp;
        }

        public static void RaiseBookExp(KungfuSkillLvData __instance, ref float value)
        {
            if (!SkillExpEnabled || _fillingExp || __instance == null)
                return;
            float old = __instance.bookExp;
            if (value <= old + 0.0001f)
                return;
            if (old <= 0f && _expGainDepth <= 0)
                return;
            if (!IsTraineeSkill(__instance))
                return;
            FillToBreakthrough(__instance);
            if (__instance.bookExp > value)
                value = __instance.bookExp;
        }

        static float FavorCap(HeroData hero, float hinted)
        {
            float cap = hinted > 0f ? hinted : 0f;
            try
            {
                float wide = hero.GetMaxFavor(100000f);
                if (wide > 0f && wide < 100000f && wide > cap)
                    cap = wide;
                if (cap <= 0f)
                {
                    float fromHint = hero.GetMaxFavor(hinted > 0f ? hinted : hero.favor);
                    if (fromHint > 0f && fromHint < 100000f)
                        cap = fromHint;
                }
            }
            catch
            {
                return cap;
            }

            if (cap <= 0f)
            {
                try
                {
                    int testCap = GlobalData.PlayTestMaxFavor;
                    if (testCap > 0)
                        cap = testCap;
                }
                catch
                {
                    return cap;
                }
            }
            return cap;
        }

        public static void EnterFavor(HeroData __instance, ref float num, ref float maxFavor, ref float __state)
        {
            __state = 0f;
            if (!MaxFavorEnabled || __instance == null || num == 0f || IsPlayerHero(__instance))
                return;
            float cap = FavorCap(__instance, maxFavor);
            if (cap <= 0f)
            {
                if (num < 0f)
                    num = 0f;
                return;
            }
            __state = cap;
            num = __instance.favor < cap ? cap - __instance.favor : 0f;
            if (maxFavor < cap)
                maxFavor = cap;
        }

        public static void LeaveFavor(HeroData __instance, float __state)
        {
            if (!MaxFavorEnabled || __instance == null || __state <= 0f)
                return;
            if (__instance.favor < __state)
                __instance.favor = __state;
        }

        public static void EnterMeetFavor(HeroData __instance, ref float startFavor)
        {
            if (!MaxFavorEnabled || __instance == null || IsPlayerHero(__instance))
                return;
            float cap = FavorCap(__instance, startFavor);
            if (cap > startFavor)
                startFavor = cap;
        }

        static bool IsPlayerEquipment(HeroEquipmentData equipment)
        {
            var player = CurrentPlayer();
            return player != null && equipment != null && player.nowEquipment == equipment;
        }

        static bool IsPlayerBag(ItemListData list)
        {
            var player = CurrentPlayer();
            return player != null && list != null && player.itemListData == list;
        }

        static bool IsPlayerBookStorage(ItemListData list)
        {
            if (list == null)
                return false;
            RefreshPlayerForce();
            var force = PlayerForce();
            return force != null && force.bookStorage == list;
        }

        static void ApplyBattle()
        {
            try
            {
                var battle = BattleController.Instance;
                if (battle == null || battle.teams == null)
                    return;
                var state = battle.battleState;
                if (state != BattleState.Fighting && state != BattleState.Ready && state != BattleState.Prepare)
                    return;

                float actionFull = GlobalData.MaxFightMovePower;
                var teams = battle.teams;
                for (int i = 0; i < teams.Count; i++)
                {
                    var team = teams[i];
                    if (team == null || team.battleUnits == null || !IsPlayerTeam(battle, team))
                        continue;
                    var units = team.battleUnits;
                    for (int u = 0; u < units.Count; u++)
                    {
                        var unit = units[u];
                        if (unit == null || !unit.IsAlive)
                            continue;
                        if (InfiniteHp)
                            FillHp(unit.heroData);
                        if (InfiniteMana)
                            FillMana(unit.heroData);
                        if (InfiniteStamina)
                            FillStamina(unit.heroData);
                        ReadyAllySkills(unit.heroData);
                        if (InfiniteAction && actionFull > 0f && unit.battleMove < actionFull)
                            unit.battleMove = actionFull;
                    }
                }
            }
            catch (Exception ex)
            {
                if (_instance != null)
                    _instance.LoggerInstance.Error($"战斗强化失败: {ex.Message}");
            }
        }

        static bool NeedsBattleTick()
        {
            return InfiniteHp || InfiniteMana || InfiniteStamina || InfiniteAction || BuffChargeEnabled || NoSkillCdEnabled;
        }

        static void FillHp(HeroData hero)
        {
            if (hero == null)
                return;
            float hp = hero.realMaxHp > 0f ? hero.realMaxHp : hero.maxhp;
            if (hp <= 0f)
                return;
            hero.hp = hp;
            hero.shownHp = hero.hp;
            hero.shownMaxHp = hero.maxhp;
            hero.shownRealMaxHp = hero.realMaxHp;
        }

        static void FillMana(HeroData hero)
        {
            if (hero == null)
                return;
            float mana = hero.realMaxMana > 0f ? hero.realMaxMana : hero.maxMana;
            if (mana <= 0f)
                return;
            hero.mana = mana;
            hero.shownMana = hero.mana;
            hero.shownMaxMana = hero.maxMana;
            hero.shownRealMaxMana = hero.realMaxMana;
        }

        static void FillStamina(HeroData hero)
        {
            if (hero == null)
                return;
            float power = hero.realMaxPower > 0f ? hero.realMaxPower : hero.maxPower;
            if (power <= 0f)
                return;
            hero.power = power;
            hero.shownPower = hero.power;
            hero.shownMaxPower = hero.maxPower;
            hero.shownRealMaxPower = hero.realMaxPower;
        }

        static void ReadyAllySkills(HeroData hero)
        {
            if (hero == null)
                return;
            if (BuffChargeEnabled)
            {
                FillBuffCharge(hero.internalSkill);
                FillBuffCharge(hero.dodgeSkill);
                FillBuffCharge(hero.uniqueSkill);
            }
            if (!NoSkillCdEnabled)
                return;
            var attacks = hero.attackSkills;
            if (attacks == null)
                return;
            for (int i = 0; i < attacks.Count; i++)
                ClearAttackCooldown(attacks[i]);
        }

        static bool _fillingSkillPower;

        static void FillBuffCharge(KungfuSkillLvData skill)
        {
            if (skill == null || _fillingSkillPower)
                return;
            float max;
            try
            {
                max = skill.MaxPower();
            }
            catch
            {
                return;
            }
            if (max <= 0f || skill.power >= max)
                return;
            _fillingSkillPower = true;
            try
            {
                skill.power = max;
            }
            finally
            {
                _fillingSkillPower = false;
            }
        }

        static void ClearAttackCooldown(KungfuSkillLvData skill)
        {
            if (skill != null && skill.cdTimeLeft > 0f)
                skill.cdTimeLeft = 0f;
        }

        static bool IsBuffSkill(HeroData hero, KungfuSkillLvData skill)
        {
            return hero != null && skill != null && (skill == hero.internalSkill || skill == hero.dodgeSkill || skill == hero.uniqueSkill);
        }

        static bool IsAttackSkill(HeroData hero, KungfuSkillLvData skill)
        {
            if (hero == null || skill == null || IsBuffSkill(hero, skill))
                return false;
            var attacks = hero.attackSkills;
            if (attacks == null)
                return false;
            for (int i = 0; i < attacks.Count; i++)
            {
                if (attacks[i] == skill)
                    return true;
            }
            return false;
        }

        static bool IsBattleAlly(HeroData hero)
        {
            if (hero == null)
                return false;
            var battle = BattleController.Instance;
            if (battle == null || battle.teams == null)
                return false;
            var state = battle.battleState;
            if (state != BattleState.Fighting && state != BattleState.Ready && state != BattleState.Prepare)
                return false;
            var teams = battle.teams;
            for (int i = 0; i < teams.Count; i++)
            {
                var team = teams[i];
                if (team == null || team.battleUnits == null || !IsPlayerTeam(battle, team))
                    continue;
                var units = team.battleUnits;
                for (int u = 0; u < units.Count; u++)
                {
                    var unit = units[u];
                    if (unit != null && unit.heroData == hero)
                        return true;
                }
            }
            return false;
        }

        public static void KeepBuffCharged(KungfuSkillLvData __instance)
        {
            if (!BuffChargeEnabled || __instance == null)
                return;
            HeroData hero;
            try
            {
                hero = __instance.GetBelongHero();
            }
            catch
            {
                return;
            }
            if (!IsBattleAlly(hero) || !IsBuffSkill(hero, __instance))
                return;
            FillBuffCharge(__instance);
        }

        public static void RefillAllyBuffCharge(HeroData __instance)
        {
            if (!BuffChargeEnabled || !IsBattleAlly(__instance))
                return;
            FillBuffCharge(__instance.internalSkill);
            FillBuffCharge(__instance.dodgeSkill);
            FillBuffCharge(__instance.uniqueSkill);
        }

        public static void ZeroAttackCd(KungfuSkillLvData __instance, ref float value)
        {
            if (!NoSkillCdEnabled || value <= 0f || __instance == null)
                return;
            HeroData hero;
            try
            {
                hero = __instance.GetBelongHero();
            }
            catch
            {
                return;
            }
            if (IsBattleAlly(hero) && IsAttackSkill(hero, __instance))
                value = 0f;
        }

        static bool IsPlayerTeam(BattleController battle, BattleTeam team)
        {
            if (team.havePlayer)
                return true;
            try
            {
                return team.ID == battle.GetPlayerControlTeamID();
            }
            catch
            {
                return false;
            }
        }

        static bool IsAllyUnit(BattleUnit unit)
        {
            if (unit == null)
                return false;
            if (unit.playerControl)
                return true;
            var battle = BattleController.Instance;
            return battle != null && unit.battleTeam != null && IsPlayerTeam(battle, unit.battleTeam);
        }

        static bool IsAllyHero(HeroData hero)
        {
            if (hero == null)
                return false;
            var battle = BattleController.Instance;
            if (battle == null || battle.teams == null)
                return false;
            if (battle.battleState != BattleState.Fighting && battle.battleState != BattleState.Ready)
                return false;
            var teams = battle.teams;
            for (int i = 0; i < teams.Count; i++)
            {
                var team = teams[i];
                if (team == null || team.battleUnits == null || !IsPlayerTeam(battle, team))
                    continue;
                var units = team.battleUnits;
                for (int u = 0; u < units.Count; u++)
                {
                    var unit = units[u];
                    if (unit != null && unit.heroData == hero)
                        return true;
                }
            }
            return false;
        }

        static int ApplyResearch()
        {
            try
            {
                RefreshPlayerForce();
                var force = PlayerForce();
                var tech = force != null ? force.GetNowResearchTech() : null;
                if (force == null || tech == null || force.nowResearchTech == 0)
                    return 0;

                float speed = force.GetResearchSpeedRate();
                int left = RealResearchDays(tech, speed);
                if (left <= TargetDays)
                    return 0;

                float perDay = tech.GetEachDayResearchPercent(speed);
                if (perDay <= 0f)
                    return 0;

                tech.researchPercent += perDay * (left - TargetDays);
                for (int i = 0; i < 8 && RealResearchDays(tech, speed) > TargetDays; i++)
                    tech.researchPercent += perDay;
                if (RealResearchDays(tech, speed) < TargetDays)
                    tech.researchPercent -= perDay;

                TryRefreshResearchUi();
                return 1;
            }
            catch (Exception ex)
            {
                if (_instance != null)
                    _instance.LoggerInstance.Error($"缩短科研失败: {ex.Message}");
                return 0;
            }
        }

        static int RealResearchDays(ForceTechLvData tech, float speed)
        {
            _readingRealResearchDays = true;
            try
            {
                return tech.GetResearchLeftDay(speed);
            }
            finally
            {
                _readingRealResearchDays = false;
            }
        }

        static ForceData PlayerForce()
        {
            if (!_hasPlayerForce)
                return null;
            var data = GameDataController.Instance;
            var world = data != null && data.gameSaveData != null ? data.gameSaveData.WorldData : null;
            var forces = world != null ? world.Forces : null;
            if (forces == null)
                return null;
            for (int i = 0; i < forces.Count; i++)
            {
                var force = forces[i];
                if (force != null && force.forceID == _playerForceId)
                    return force;
            }
            return null;
        }

        static bool IsPlayerCurrentResearch(ForceTechLvData tech)
        {
            if (tech == null || !_hasPlayerForce)
                return false;
            var force = PlayerForce();
            return force != null && force.nowResearchTech != 0 && force.nowResearchTech == tech.techID;
        }

        static int ClampArea(AreaData area, bool owned)
        {
            int changed = 0;
            var tiles = area.areaTiles;
            if (tiles != null)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    var tile = tiles[i];
                    if (tile == null)
                        continue;

                    var building = tile.building;
                    if (building != null && (owned || IsBusyPrivateHouse(building)))
                        changed += ClampBuilding(building);

                    if (!owned)
                        continue;

                    var road = tile.areaRoadData;
                    if (road != null && road.upgradeTimeLeft > TargetDays)
                    {
                        road.upgradeTimeLeft = TargetDays;
                        changed++;
                    }
                }
            }

            if (!owned)
                return changed;

            var defence = area.areaBranchDefenceUpgradeLeftTime;
            if (defence != null)
            {
                for (int i = 0; i < defence.Count; i++)
                {
                    if (defence[i] > TargetDays)
                    {
                        defence[i] = TargetDays;
                        changed++;
                    }
                }
            }

            return changed;
        }

        static int ClampBuilding(AreaBuildingData building)
        {
            int changed = 0;
            if (building.buildTimeLeft > TargetDays)
            {
                building.buildTimeLeft = TargetDays;
                changed++;
            }
            if (building.upgradeTimeLeft > TargetDays)
            {
                building.upgradeTimeLeft = TargetDays;
                changed++;
            }
            if (building.destroyTimeLeft > TargetDays)
            {
                building.destroyTimeLeft = TargetDays;
                changed++;
            }
            return changed;
        }

        static bool IsBusyPrivateHouse(AreaBuildingData building)
        {
            if (building.buildTimeLeft <= TargetDays
                && building.upgradeTimeLeft <= TargetDays
                && building.destroyTimeLeft <= TargetDays)
                return false;
            return IsPrivateHouse(building);
        }

        static bool IsPrivateHouse(AreaBuildingData building)
        {
            if (building == null)
                return false;
            if (_playerHeroId > 0 && building.belongHeroID == _playerHeroId)
                return true;
            try
            {
                var data = building.DataBase();
                if (data != null && IsPrivateHouseName(data.name))
                    return true;
            }
            catch
            {
            }

            try
            {
                return IsPrivateHouseName(building.Name(false));
            }
            catch
            {
                return false;
            }
        }

        static bool IsPrivateHouseName(string name)
        {
            return !string.IsNullOrEmpty(name) && name.Contains("私宅");
        }

        static void RefreshPlayerForce()
        {
            _hasPlayerForce = false;
            _playerForceId = 0;
            _playerHeroId = 0;
            try
            {
                var data = GameDataController.Instance;
                var world = data != null && data.gameSaveData != null ? data.gameSaveData.WorldData : null;
                var player = world != null ? world.Player() : null;
                if (player == null)
                    return;
                _playerHeroId = player.heroID;
                _playerForceId = player.belongForceID;
                _hasPlayerForce = _playerForceId > 0;
            }
            catch
            {
                _hasPlayerForce = false;
                _playerHeroId = 0;
            }
        }

        static bool Owns(AreaData area)
        {
            if (area == null)
                return false;
            try
            {
                if (area.BelongPlayer())
                    return true;
            }
            catch
            {
            }

            return _hasPlayerForce && area.belongForceID == _playerForceId;
        }

        static AreaData ResolveArea(object instance, object[] args)
        {
            try
            {
                if (instance is AreaBuildingData building)
                    return building.GetArea();
                if (instance is AreaRoadData road)
                    return road.GetArea();
                if (instance is AreaData area)
                    return area;
                if (args == null)
                    return null;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] is AreaData fromArg)
                        return fromArg;
                }
            }
            catch
            {
            }

            return null;
        }

        static void TryRefreshBuildingUi()
        {
            try
            {
                var ui = BuildingUIController.Instance;
                if (ui != null)
                    ui.RefreshBuildingUI();
            }
            catch
            {
            }
        }

        static void TryRefreshResearchUi()
        {
            try
            {
                var ui = ResearchUIController.Instance;
                if (ui != null)
                    ui.RefreshNowResearch();
            }
            catch
            {
            }
        }
    }
}
