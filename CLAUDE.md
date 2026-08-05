# PushUp Arena

Фитнес-RPG (сингл-плеер, Android, портрет, **без мультиплеера — никогда**): игрок сражается с монстрами в данже, урон наносится реальными отжиманиями (позже — другие упражнения и трекинг камерой). Unity 6000.0.60f1, URP, Zenject. Проект личный — для Ивана и его друзей, без монетизации.

## Документация (Docs/ — вики проекта)

| Файл | Что там |
|---|---|
| [Docs/GDD.md](Docs/GDD.md) | Гейм-дизайн: концепция, режимы, формула 500 уровней, UI по референсам |
| [Docs/STORY.md](Docs/STORY.md) | Сюжет: Крепград, Великая Лень, 5 актов по тирам монстров |
| [Docs/PLAN.md](Docs/PLAN.md) | План реализации со статусами, принятые техрешения |
| [Docs/ARCHITECTURE.md](Docs/ARCHITECTURE.md) | Архитектура: слои, правила кода, как добавлять фичи |
| [Docs/IDEAS.md](Docs/IDEAS.md) | Бэклог идей: Ивана + 15 идей Claude (стрики, данж дня, экипировка…) |
| [Docs/QUESTIONS.md](Docs/QUESTIONS.md) | Вопросы к Ивану + решения, принятые самостоятельно |
| Docs/References/ | Референс-скриншоты UI (положить сюда) |

## Реализованные фичи → код

| Фича | Код | Статус |
|---|---|---|
| Генератор 500 уровней (HP 3→20 волнами, боссы) | `Assets/Scripts/Game/Levels/` | ✅ |
| Конфиги (SO, авто-создание ассетов): монстры, кампания, трекинг, упражнения | `Game/Monsters/`, `Game/Campaigns/`, `Game/Reps/TrackingConfig.cs`, `Game/Exercises/` | ✅ |
| Имена монстров (5 тиров + префиксы + 50 боссов, иконки) | `Game/Monsters/` + `Services/MonsterNameService.cs` | ✅ |
| Бой (HP, повторения, победа) | `Game/Battle/Services/BattleService.cs` | ✅ |
| Кампания (прогресс, круги-prestige, награды) | `Game/Campaign/Services/CampaignService.cs` | ✅ |
| Счёт повторений: кнопка «+1» + камера (детект по затемнению кадра, TrackingConfig) | `Game/Reps/` | ✅ MVP |
| Статистика по дням и упражнениям + экран | `Game/Stats/`, `Game/Exercises/`, `Game/UI/UIStatsScreen.cs` | ✅ |
| Золото (награда за монстров, боссы ×2, кошелёк) | `Game/Wallet/Services/WalletService.cs` | ✅ |
| Треки кампании (данж + лестница «Просто отжиматься», упражнение и золото per-трек в конфиге) | `Game/Campaign/` (CampaignConfig.Tracks) | ✅ |
| Победа тостом + авто-следующий враг | `Game/UI/UIToastView.cs` | ✅ |
| Двухфазный урон (pending при глубине, фиксация при подъёме, откат при кривой технике) | `Game/Battles/`, `Game/Poses/` | ✅ |
| Звуки: лёг/встал/отмена (синтез кодом, клипы через SoundConfig) | `Game/Sounds/` | ✅ |
| Сейв (PlayerPrefs + Newtonsoft JSON, Version=3, автосейв) | `Assets/Scripts/Common/Save/` | ✅ |
| UI кодом без префабов (хаб-меню, бой, статистика, попапы) | `Game/UI/`, `Common/UI/` | ✅ |
| Сцена/DI/конфиги/иконки — авто-создание в редакторе | `Assets/Scripts/Infrastructure/Editor/` | ✅ |
| AI-иконки монстров (Pollinations/FLUX, бесплатно) | `Tools/gen-monsters.ps1` → `Assets/Art/Monsters/` | ✅ |
| Билд APK одним кликом | меню `Tools/PushUp/Build Android APK` | ✅ |
| Честный трекинг: Sentis + pose, углы суставов, проверка техники, скелет-оверлей | `Game/Pose/` + `Game/UI/UISkeletonView.cs` | ✅ код (нужна модель в `Resources/Models/MoveNet`) |
| Анлоки/челленджи за золото | — | ⏳ Иван придумает |

## Правила кода (обязательны)

Архитектура и стиль — по образцу Elements (`C:\Work\Elements\Elements`), детали в [Docs/ARCHITECTURE.md](Docs/ARCHITECTURE.md):

- namespace = путь под `Assets/Scripts/`; один тип на файл; **никаких комментариев в `.cs`**.
- **Папки фич в `Game/` — во множественном числе** (Battles, Campaigns, Monsters, Poses, Wallets…); `Common` — как есть.
- Суффиксы: `*Config` (ScriptableObject), `*State` (сериализуемое в сейв), `*Data` (payload), `*Service` (логика, не MonoBehaviour), `*Provider`, `*Factory`, `UI*` (тонкие вьюхи), `Proxy*` (статика чистых функций). Запрещены `*Manager`, `*Helper`, `*Catalog`.
- Пайплайн фичи: **Config → State → Service → View**. Логика никогда не живёт во вьюхах; вьюхи не опрашивают сервисы из `Update()`.
- Только конструкторная инъекция (Zenject); порядок старта — явные `Init()` в `GameEntrypoint`; подписался — отпишись в `Dispose()`.
- **Баланс и контент — только через конфиги** (MonstersConfig, CampaignConfig), не через константы в коде. Новая фича = новый конфиг, если есть что крутить.
- Расширяемость важнее краткости: упражнения за `IRepCounter`/`ProxyExerciseIds`, источники повторений подменяемы, генерация уровней детерминирована.
- Не ломать сейвы молча: новое поле в `GameState` — с дефолтом; несовместимое изменение — поднять `Version` и написать миграцию.
- Ошибки не роняют игру: `Debug.LogError` + безопасный выход вместо `throw`. Guard-clause'ы вместо вложенности.
- Перед завершением работы — проверить компиляцию (рецепт: csproj с референсами на Unity DLL, см. память/`Docs/PLAN.md`).

## Процесс

- 🚫 **Git-коммиты запрещены** — Иван просматривает и коммитит сам.
- Вопросы/сомнения → [Docs/QUESTIONS.md](Docs/QUESTIONS.md): записать вопрос, принять решение самому, продолжить. Иван потом пересмотрит.
- Идеи Ивана → [Docs/IDEAS.md](Docs/IDEAS.md); реализованное отмечать ✅ и переносить суть в GDD.
- Новая фича → строка в таблице «фичи → код» здесь + отметка в PLAN.md. Фича без записи в доках не считается сделанной.

## Инструменты

- **Unity MCP** подключён в `.mcp.json` + пакет `com.coplaydev.unity-mcp` в manifest (как в Elements/WelcomeMobile). Если MCP-тулы недоступны в сессии — попросить Ивана подключить.
- **Генерация картинок**: `Tools/gen-image.ps1` (одна картинка, Pollinations.ai/FLUX, бесплатно, без ключей), `Tools/gen-monsters.ps1` (батч всех иконок монстров, resume-able). Имена файлов `tier{t}_{ii}.png`/`boss_{ii}.png` автоматически подхватываются в MonstersConfig (`Tools/PushUp/Assign Monster Icons`, срабатывает и само при загрузке редактора). `Tools/cutbg.py` — вырезание белого фона в alpha, если понадобится.
- **Editor-меню** `Tools/PushUp/…`: Create Game Scene, Create Configs, Assign Monster Icons, Build Android APK, **Add Rep (F5 в Play-режиме — нанести урон для теста)**. Сцена и конфиги создаются автоматически при загрузке редактора, если их нет.
- **Unity MCP работает**: рефреш/консоль/Play/execute_code. ⚠️ В execute_code запрещены циклы «до игрового состояния» — победа авто-запускает следующего монстра, цикл зависнет (см. память editor-code-safety).

## Как запустить

Переключиться в окно Unity (перекомпилируется, сцена/конфиги создадутся сами) → открыть `Assets/Scenes/Game.unity` → Play. Билд на телефон: `Tools/PushUp/Build Android APK` → `Builds/PushUpArena.apk`.
