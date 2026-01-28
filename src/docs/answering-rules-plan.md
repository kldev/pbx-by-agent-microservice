# Plan: Mikroserwis Answering Rules

## 1. Opis funkcjonalny

### 1.1 Cel
Mikroserwis do zarządzania regułami odpowiadania na połączenia przychodzące (inbound) dla kont SIP. Umożliwia konfigurację automatycznych akcji w zależności od dnia tygodnia i przedziału czasowego.

### 1.2 Główne wymagania
- Reguły czasowe z granulacją od 15 minut do całego tygodnia
- Obsługa tylko połączeń **inbound**
- Konfiguracja per konto SIP
- Dostępne akcje:
  - **Voicemail** - przekierowanie do poczty głosowej
  - **Redirect** - przekierowanie na inny numer
  - **RedirectToGroup** - przekierowanie do grupy (hunt group / ring group)
  - **DisconnectWithVoicemessage** - rozłączenie z odtworzeniem komunikatu głosowego
- Opcjonalna notyfikacja email przy zastosowaniu reguły

---

## 2. Model danych

### 2.1 Encje

#### AnsweringRule (główna encja reguły)
```
AnsweringRule : BaseAuditableTable
├── SipAccountGid: string           # Gid konta SIP (32 znaki)
├── Name: string                    # Nazwa reguły (np. "Poza godzinami pracy")
├── Description: string?            # Opcjonalny opis
├── Priority: int                   # Priorytet (niższa wartość = wyższy priorytet)
├── IsEnabled: bool                 # Czy reguła jest aktywna
├── ActionType: AnsweringRuleAction # Enum: Voicemail, Redirect, RedirectToGroup, DisconnectWithVoicemessage
├── ActionTarget: string?           # Numer docelowy (dla Redirect) lub Gid grupy (dla RedirectToGroup)
├── VoicemailBoxGid: string?        # Gid skrzynki voicemail (dla Voicemail)
├── VoiceMessageGid: string?        # Gid komunikatu głosowego (dla DisconnectWithVoicemessage)
├── SendEmailNotification: bool     # Czy wysłać email przy zastosowaniu
├── NotificationEmail: string?      # Email do powiadomień (jeśli inny niż domyślny)
└── TimeSlots: ICollection<AnsweringRuleTimeSlot>
```

#### AnsweringRuleTimeSlot (przedziały czasowe)
```
AnsweringRuleTimeSlot : BaseTable
├── AnsweringRuleId: long          # FK do reguły
├── DayOfWeek: DayOfWeek           # 0=Niedziela, 1=Poniedziałek, ..., 6=Sobota
├── StartTime: TimeOnly            # Czas rozpoczęcia (00:00 - 23:45)
├── EndTime: TimeOnly              # Czas zakończenia (00:15 - 24:00)
├── IsAllDay: bool                 # Cały dzień (00:00 - 24:00)
└── AnsweringRule: AnsweringRule   # Navigation
```

**Uwagi:**
- Granulacja czasu: 15 minut (walidacja: StartTime i EndTime muszą być wielokrotnością 15 minut)
- EndTime może być 24:00 (reprezentowane jako 23:59:59 lub specjalna wartość)
- Reguła może mieć wiele TimeSlots (np. Pon-Pt 18:00-08:00)

### 2.2 Enumy

#### AnsweringRuleAction
```csharp
public enum AnsweringRuleAction
{
    Voicemail = 1,
    Redirect = 2,
    RedirectToGroup = 3,
    DisconnectWithVoicemessage = 4
}
```

### 2.3 Diagram relacji
```
┌─────────────────┐       ┌────────────────────────┐
│   SipAccount    │       │    AnsweringRule       │
│   (zewnętrzna)  │1─────*│                        │
└─────────────────┘       │  - Name                │
                          │  - ActionType          │
                          │  - ActionTarget        │
                          │  - SendEmailNotification│
                          └───────────┬────────────┘
                                      │1
                                      │
                                      │*
                          ┌───────────┴────────────┐
                          │ AnsweringRuleTimeSlot  │
                          │                        │
                          │  - DayOfWeek           │
                          │  - StartTime           │
                          │  - EndTime             │
                          │  - IsAllDay            │
                          └────────────────────────┘
```

---

## 3. Struktura projektu

### 3.1 Katalogi
```
src/backend/Services/AnswerRule/
├── add-mig.sh                       # Skrypt do tworzenia migracji EF
├── AnswerRule.Api/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Infrastructure/
│   │   ├── DependencyInjection.cs
│   │   ├── EndpointExtensions.cs
│   │   └── DatabaseExtensions.cs
│   ├── Features/
│   │   └── Rules/
│   │       ├── Endpoint.cs
│   │       ├── IRuleService.cs
│   │       ├── RuleService.cs
│   │       ├── IRuleDataHandler.cs
│   │       ├── RuleDataHandler.cs
│   │       └── Model/
│   │           ├── AnsweringRuleResponse.cs
│   │           ├── AnsweringRuleDetailResponse.cs
│   │           ├── CreateAnsweringRuleRequest.cs
│   │           ├── UpdateAnsweringRuleRequest.cs
│   │           ├── AnsweringRuleListFilter.cs
│   │           ├── TimeSlotDto.cs
│   │           ├── CheckRuleRequest.cs
│   │           └── CheckRuleResponse.cs
│   ├── Seed/
│   │   ├── FixedGuids.cs
│   │   ├── ShowcaseData.cs
│   │   └── SeedService.cs
│   └── Definitions/
│       ├── ErrorCodes.cs
│       └── ErrorCodeHelper.cs
├── AnswerRule.Data/
│   ├── AnswerRuleDbContext.cs
│   ├── Entities/
│   │   ├── AnsweringRule.cs
│   │   └── AnsweringRuleTimeSlot.cs
│   ├── Enums/
│   │   └── AnsweringRuleAction.cs
│   ├── Configurations/
│   │   ├── AnsweringRuleConfiguration.cs
│   │   └── AnsweringRuleTimeSlotConfiguration.cs
│   └── Migrations/
└── AnswerRule.Api.Tests/
    ├── Infrastructure/
    │   ├── AnswerRuleMySqlTestContainer.cs
    │   ├── AnswerRuleDatabaseCollection.cs
    │   ├── AnswerRuleApplicationFactory.cs
    │   └── AnswerRuleTestDataSeeder.cs
    └── RulesEndpointTests.cs
```

---

## 4. API Endpoints

> **Uwaga:** Mikroserwis używa prefixu `/api/*`. Gateway (YARP) automatycznie mapuje:
> - Request: `/api/answerrule/rules/list` → Mikroserwis: `/api/rules/list`
> - Transform w YARP: `PathPattern: "/api/{**catch-all}"`

### 4.1 CRUD Reguł

| Metoda | Endpoint (mikroserwis) | Endpoint (przez Gateway) | Opis |
|--------|------------------------|--------------------------|------|
| POST | `/api/rules/list` | `/api/answerrule/rules/list` | Lista reguł (z filtrowaniem i paginacją) |
| GET | `/api/rules/{gid}` | `/api/answerrule/rules/{gid}` | Pobierz regułę po GID (z TimeSlots) |
| POST | `/api/rules` | `/api/answerrule/rules` | Utwórz nową regułę |
| PUT | `/api/rules/{gid}` | `/api/answerrule/rules/{gid}` | Aktualizuj regułę |
| DELETE | `/api/rules/{gid}` | `/api/answerrule/rules/{gid}` | Usuń regułę (soft delete) |
| PATCH | `/api/rules/{gid}/toggle` | `/api/answerrule/rules/{gid}/toggle` | Włącz/wyłącz regułę |
| POST | `/api/rules/{gid}/duplicate` | `/api/answerrule/rules/{gid}/duplicate` | Duplikuj regułę |

### 4.2 Operacje na TimeSlots

| Metoda | Endpoint (mikroserwis) | Endpoint (przez Gateway) | Opis |
|--------|------------------------|--------------------------|------|
| POST | `/api/rules/{ruleGid}/timeslots` | `/api/answerrule/rules/{ruleGid}/timeslots` | Dodaj TimeSlot do reguły |
| PUT | `/api/rules/{ruleGid}/timeslots/{slotGid}` | `/api/answerrule/rules/{ruleGid}/timeslots/{slotGid}` | Aktualizuj TimeSlot |
| DELETE | `/api/rules/{ruleGid}/timeslots/{slotGid}` | `/api/answerrule/rules/{ruleGid}/timeslots/{slotGid}` | Usuń TimeSlot |

### 4.3 Sprawdzanie aktywnej reguły (dla silnika routingu)

| Metoda | Endpoint (mikroserwis) | Endpoint (przez Gateway) | Opis |
|--------|------------------------|--------------------------|------|
| POST | `/api/check` | `/api/answerrule/check` | Sprawdź aktywną regułę dla konta SIP w danym momencie |

**CheckRuleRequest:**
```json
{
  "sipAccountGid": "abc123",
  "checkDateTime": "2024-01-15T14:30:00Z"  // opcjonalne, domyślnie teraz
}
```

**CheckRuleResponse:**
```json
{
  "hasActiveRule": true,
  "rule": {
    "gid": "rule123",
    "name": "Poza godzinami",
    "actionType": "Voicemail",
    "actionTarget": null,
    "voicemailBoxGid": "vm00100010001000100010001000001",
    "voiceMessageGid": null,
    "sendEmailNotification": true,
    "notificationEmail": "user@example.com"
  }
}
```

---

## 5. Modele DTO

### 5.1 Request DTOs

**CreateAnsweringRuleRequest:**
```csharp
public class CreateAnsweringRuleRequest
{
    public string SipAccountGid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Priority { get; set; } = 100;
    public bool IsEnabled { get; set; } = true;
    public AnsweringRuleAction ActionType { get; set; }
    public string? ActionTarget { get; set; }
    public string? VoicemailBoxGid { get; set; }
    public string? VoiceMessageGid { get; set; }
    public bool SendEmailNotification { get; set; }
    public string? NotificationEmail { get; set; }
    public List<TimeSlotDto> TimeSlots { get; set; } = [];
}
```

**TimeSlotDto:**
```csharp
public class TimeSlotDto
{
    public string? Gid { get; set; }  // null dla nowych
    public DayOfWeek DayOfWeek { get; set; }
    public string StartTime { get; set; } = null!;  // "HH:mm" format
    public string EndTime { get; set; } = null!;    // "HH:mm" format
    public bool IsAllDay { get; set; }
}
```

**AnsweringRuleListFilter:**
```csharp
public class AnsweringRuleListFilter : PagedRequest
{
    public string? SipAccountGid { get; set; }  // wymagane
    public string? Search { get; set; }
    public bool? IsEnabled { get; set; }
    public AnsweringRuleAction? ActionType { get; set; }
}
```

### 5.2 Response DTOs

**AnsweringRuleResponse (lista):**
```csharp
public class AnsweringRuleResponse
{
    public string Gid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public bool IsEnabled { get; set; }
    public AnsweringRuleAction ActionType { get; set; }
    public string? ActionTarget { get; set; }
    public bool SendEmailNotification { get; set; }
    public int TimeSlotsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**AnsweringRuleDetailResponse (szczegóły):**
```csharp
public class AnsweringRuleDetailResponse : AnsweringRuleResponse
{
    public string SipAccountGid { get; set; } = null!;
    public string? VoicemailBoxGid { get; set; }
    public string? VoiceMessageGid { get; set; }
    public string? NotificationEmail { get; set; }
    public List<TimeSlotDto> TimeSlots { get; set; } = [];
}
```

---

## 6. Logika biznesowa

### 6.1 Walidacje

**Reguła:**
- `Name` - wymagane, max 100 znaków
- `SipAccountGid` - wymagane, musi istnieć
- `Priority` - wymagane, >= 0
- `ActionType` - wymagane
- `ActionTarget` - wymagane dla Redirect i RedirectToGroup
- `VoicemailBoxGid` - wymagane dla Voicemail
- `VoiceMessageGid` - wymagane dla DisconnectWithVoicemessage
- `NotificationEmail` - format email jeśli podany

**TimeSlot:**
- `DayOfWeek` - 0-6
- `StartTime` - wielokrotność 15 minut (00, 15, 30, 45)
- `EndTime` - wielokrotność 15 minut, > StartTime (lub 24:00)
- Brak nakładających się TimeSlots w obrębie tej samej reguły
- Reguła musi mieć co najmniej jeden TimeSlot

### 6.2 Algorytm dopasowania reguły

```
1. Pobierz wszystkie aktywne (IsEnabled=true) reguły dla SipAccountGid
2. Dla każdej reguły sprawdź TimeSlots:
   - Czy dzień tygodnia pasuje do DayOfWeek?
   - Czy czas mieści się między StartTime a EndTime?
3. Jeśli wiele reguł pasuje - wybierz tę z najniższym Priority
4. Jeśli ten sam Priority - wybierz najnowszą (CreatedAt DESC)
5. Zwróć regułę lub null
```

### 6.3 Obsługa TimeSlots

**Tryb "Cały tydzień":**
- Można utworzyć regułę z 7 TimeSlots (jeden na każdy dzień) z IsAllDay=true
- Lub jeden TimeSlot per dzień z konkretnymi godzinami

**Nakładające się przedziały:**
- System NIE pozwala na nakładające się TimeSlots w jednej regule
- Ale różne reguły MOGĄ mieć nakładające się przedziały (rozstrzyga Priority)

---

## 7. Kody błędów

```csharp
public static class ErrorCodes
{
    public static class Rule
    {
        [ErrorMessage("Reguła nie została znaleziona")]
        public const string NotFound = "rule.not_found";

        [ErrorMessage("Nazwa reguły jest wymagana")]
        public const string NameRequired = "rule.name_required";

        [ErrorMessage("Konto SIP jest wymagane")]
        public const string SipAccountRequired = "rule.sip_account_required";

        [ErrorMessage("Konto SIP nie istnieje")]
        public const string SipAccountNotFound = "rule.sip_account_not_found";

        [ErrorMessage("Cel akcji jest wymagany dla tego typu reguły")]
        public const string ActionTargetRequired = "rule.action_target_required";

        [ErrorMessage("Skrzynka voicemail jest wymagana dla tego typu reguły")]
        public const string VoicemailBoxRequired = "rule.voicemail_box_required";

        [ErrorMessage("Komunikat głosowy jest wymagany dla tego typu reguły")]
        public const string VoiceMessageRequired = "rule.voice_message_required";

        [ErrorMessage("Reguła musi mieć co najmniej jeden przedział czasowy")]
        public const string TimeSlotRequired = "rule.timeslot_required";
    }

    public static class TimeSlot
    {
        [ErrorMessage("Przedział czasowy nie został znaleziony")]
        public const string NotFound = "timeslot.not_found";

        [ErrorMessage("Czas rozpoczęcia musi być wielokrotnością 15 minut")]
        public const string InvalidStartTimeGranularity = "timeslot.invalid_start_time_granularity";

        [ErrorMessage("Czas zakończenia musi być wielokrotnością 15 minut")]
        public const string InvalidEndTimeGranularity = "timeslot.invalid_end_time_granularity";

        [ErrorMessage("Czas zakończenia musi być większy niż czas rozpoczęcia")]
        public const string EndTimeBeforeStartTime = "timeslot.end_time_before_start_time";

        [ErrorMessage("Przedziały czasowe nie mogą się nakładać")]
        public const string OverlappingSlots = "timeslot.overlapping_slots";
    }
}
```

---

## 8. Integracja z Gateway

### 8.1 Konfiguracja YARP

**appsettings.json (Gateway):**
```json
{
  "ReverseProxy": {
    "Routes": {
      "answerrule-route": {
        "ClusterId": "answerrule-cluster",
        "AuthorizationPolicy": "default",
        "Match": {
          "Path": "/api/answerrule/{**catch-all}"
        },
        "Transforms": [
          { "PathPattern": "/api/{**catch-all}" }
        ]
      }
    },
    "Clusters": {
      "answerrule-cluster": {
        "Destinations": {
          "answerrule-destination": {
            "Address": "http://answerrule:8080"
          }
        }
      }
    }
  }
}
```

### 8.2 SwaggerAggregator
```csharp
new("AnswerRule", "/api/answerrule", "answerrule-cluster"),
```

---

## 9. Konfiguracja bazy danych

### 9.1 Tabele MySQL

**answering_rules:**
```sql
CREATE TABLE answering_rules (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    gid VARCHAR(32) NOT NULL UNIQUE,
    sip_account_gid VARCHAR(32) NOT NULL,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),
    priority INT NOT NULL DEFAULT 100,
    is_enabled TINYINT(1) NOT NULL DEFAULT 1,
    action_type INT NOT NULL,
    action_target VARCHAR(255),
    voicemail_box_gid VARCHAR(32),
    voice_message_gid VARCHAR(32),
    send_email_notification TINYINT(1) NOT NULL DEFAULT 0,
    notification_email VARCHAR(255),
    created_at DATETIME NOT NULL,
    created_by_user_id BIGINT,
    modified_at DATETIME,
    modified_by_user_id BIGINT,
    is_deleted TINYINT(1) NOT NULL DEFAULT 0,
    deleted_at DATETIME,

    INDEX ix_answering_rules_gid (gid),
    INDEX ix_answering_rules_sip_account (sip_account_gid),
    INDEX ix_answering_rules_enabled_priority (is_enabled, priority)
);
```

**answering_rule_time_slots:**
```sql
CREATE TABLE answering_rule_time_slots (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    gid VARCHAR(32) NOT NULL UNIQUE,
    answering_rule_id BIGINT NOT NULL,
    day_of_week INT NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    is_all_day TINYINT(1) NOT NULL DEFAULT 0,

    INDEX ix_time_slots_gid (gid),
    INDEX ix_time_slots_rule (answering_rule_id),
    FOREIGN KEY (answering_rule_id) REFERENCES answering_rules(id)
);
```

### 9.2 Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=pbx_answerrule;User=root;Password=password;CharSet=utf8mb4;"
  }
}
```

### 9.3 Skrypt add-mig.sh
```bash
#!/bin/bash
dotnet ef migrations add "$1" \
  --startup-project ./AnswerRule.Api/AnswerRule.Api.csproj \
  --project ./AnswerRule.Data
```

**Użycie:**
```bash
cd src/backend/Services/AnswerRule
chmod +x add-mig.sh
./add-mig.sh Initial
```

### 9.4 Migracja Initial
Pierwsza migracja `Initial` zostanie utworzona po zdefiniowaniu encji i DbContext. Zawiera:
- Tabelę `answering_rules`
- Tabelę `answering_rule_time_slots`
- Indeksy i klucze obce

---

## 10. Port serwisu

| Serwis | Port | Prefix API (mikroserwis) | Prefix API (Gateway) |
|--------|------|--------------------------|----------------------|
| AnswerRule | 5020 | `/api/*` | `/api/answerrule/*` |

---

## 11. Uprawnienia (Role)

| Endpoint (mikroserwis) | Dozwolone role |
|------------------------|----------------|
| POST `/api/rules/list` | Root, Ops, Admin, User |
| GET `/api/rules/{gid}` | Root, Ops, Admin, User |
| POST `/api/rules` | Root, Ops, Admin |
| PUT `/api/rules/{gid}` | Root, Ops, Admin |
| DELETE `/api/rules/{gid}` | Root, Ops, Admin |
| POST `/api/check` | Root, Ops (wewnętrzny, dla silnika routingu) |

**Uwaga:** Użytkownicy (User) mogą zarządzać tylko regułami dla swoich kont SIP. Weryfikacja w serwisie.

---

## 12. Przypadki użycia

### 12.1 Scenariusz: Poza godzinami pracy
**Reguła:** "Przekieruj na voicemail po 17:00 i w weekendy"
```json
{
  "sipAccountGid": "sip0000100010001000100010001001",
  "name": "Poza godzinami pracy",
  "actionType": "Voicemail",
  "voicemailBoxGid": "vm00100010001000100010001000001",
  "sendEmailNotification": true,
  "timeSlots": [
    { "dayOfWeek": 1, "startTime": "17:00", "endTime": "24:00" },
    { "dayOfWeek": 2, "startTime": "17:00", "endTime": "24:00" },
    { "dayOfWeek": 3, "startTime": "17:00", "endTime": "24:00" },
    { "dayOfWeek": 4, "startTime": "17:00", "endTime": "24:00" },
    { "dayOfWeek": 5, "startTime": "17:00", "endTime": "24:00" },
    { "dayOfWeek": 0, "isAllDay": true },
    { "dayOfWeek": 6, "isAllDay": true },
    { "dayOfWeek": 1, "startTime": "00:00", "endTime": "08:00" },
    { "dayOfWeek": 2, "startTime": "00:00", "endTime": "08:00" },
    { "dayOfWeek": 3, "startTime": "00:00", "endTime": "08:00" },
    { "dayOfWeek": 4, "startTime": "00:00", "endTime": "08:00" },
    { "dayOfWeek": 5, "startTime": "00:00", "endTime": "08:00" }
  ]
}
```

### 12.2 Scenariusz: Przerwa obiadowa
**Reguła:** "Przekieruj na grupę wsparcia podczas przerwy"
```json
{
  "sipAccountGid": "sip0000100010001000100010001001",
  "name": "Przerwa obiadowa",
  "actionType": "RedirectToGroup",
  "actionTarget": "grp0000100010001000100010001001",
  "priority": 50,
  "timeSlots": [
    { "dayOfWeek": 1, "startTime": "12:00", "endTime": "13:00" },
    { "dayOfWeek": 2, "startTime": "12:00", "endTime": "13:00" },
    { "dayOfWeek": 3, "startTime": "12:00", "endTime": "13:00" },
    { "dayOfWeek": 4, "startTime": "12:00", "endTime": "13:00" },
    { "dayOfWeek": 5, "startTime": "12:00", "endTime": "13:00" }
  ]
}
```

### 12.3 Scenariusz: Urlop
**Reguła:** "Całkowite przekierowanie przez tydzień"
```json
{
  "sipAccountGid": "sip0000100010001000100010001001",
  "name": "Urlop 15-22 stycznia",
  "description": "Przekierowanie podczas urlopu",
  "actionType": "Redirect",
  "actionTarget": "+48123456789",
  "priority": 10,
  "sendEmailNotification": true,
  "notificationEmail": "backup@company.com",
  "timeSlots": [
    { "dayOfWeek": 0, "isAllDay": true },
    { "dayOfWeek": 1, "isAllDay": true },
    { "dayOfWeek": 2, "isAllDay": true },
    { "dayOfWeek": 3, "isAllDay": true },
    { "dayOfWeek": 4, "isAllDay": true },
    { "dayOfWeek": 5, "isAllDay": true },
    { "dayOfWeek": 6, "isAllDay": true }
  ]
}
```

---

## 13. Testowanie

> **WYMAGANE:** Testy endpointów API muszą być napisane i przechodzić PRZED uruchomieniem aplikacji showcase. Bez testów nie ma pewności, że API działa poprawnie.

### 13.1 Testy integracyjne (endpointy)
| Test | Endpoint | Opis |
|------|----------|------|
| `CreateRule_ValidRequest_ReturnsCreated` | POST `/api/rules` | Tworzenie reguły z TimeSlots |
| `CreateRule_EmptyName_Returns400` | POST `/api/rules` | Walidacja - brak nazwy |
| `CreateRule_InvalidTimeGranularity_Returns400` | POST `/api/rules` | Walidacja - czas nie jest wielokrotnością 15 min |
| `CreateRule_OverlappingSlots_Returns400` | POST `/api/rules` | Walidacja - nakładające się sloty |
| `GetList_ReturnsPagedResults` | POST `/api/rules/list` | Lista z paginacją |
| `GetByGid_ExistingRule_ReturnsRule` | GET `/api/rules/{gid}` | Pobieranie reguły |
| `GetByGid_NonExisting_Returns404` | GET `/api/rules/{gid}` | Nie znaleziono |
| `UpdateRule_ValidRequest_ReturnsUpdated` | PUT `/api/rules/{gid}` | Aktualizacja |
| `DeleteRule_ExistingRule_Returns200` | DELETE `/api/rules/{gid}` | Soft delete |
| `ToggleRule_TogglesIsEnabled` | PATCH `/api/rules/{gid}/toggle` | Włącz/wyłącz |
| `CheckRule_ActiveRule_ReturnsRule` | POST `/api/check` | Dopasowanie aktywnej reguły |
| `CheckRule_NoActiveRule_ReturnsNull` | POST `/api/check` | Brak aktywnej reguły |
| `CheckRule_PriorityOrder_ReturnsHighestPriority` | POST `/api/check` | Priorytet przy wielu regułach |

### 13.2 Scenariusze testowe
1. **Happy path**: Utwórz regułę -> sprawdź czy jest aktywna w danym czasie
2. **Priorytet**: Dwie reguły nakładające się czasowo -> wybiera niższy priorytet
3. **Wyłączona reguła**: IsEnabled=false -> nie jest dopasowywana
4. **Brak TimeSlots**: Walidacja - błąd
5. **Nakładające się sloty**: Walidacja - błąd
6. **Granulacja czasu**: 14:17 -> błąd walidacji

---

## 14. Zależności zewnętrzne

### 14.1 Obecnie
- Brak bezpośrednich zależności (SipAccountId przechowywane jako long)

### 14.2 Do rozważenia w przyszłości
- Walidacja SipAccountGid przez wywołanie do serwisu kont SIP
- Walidacja VoicemailBoxGid przez wywołanie do serwisu voicemail
- Wysyłanie notyfikacji email przez serwis powiadomień

---

## 15. Checklist implementacji

### Faza 1: Infrastruktura
- [ ] Utworzenie projektów (Api, Data, Tests)
- [ ] Konfiguracja Program.cs, appsettings.json
- [ ] DependencyInjection, EndpointExtensions, DatabaseExtensions
- [ ] DbContext i encje
- [ ] Konfiguracje EF Core
- [ ] Skrypt add-mig.sh
- [ ] Migracja Initial

### Faza 2: Feature Rules
- [ ] Modele DTO (Request/Response)
- [ ] ErrorCodes i ErrorCodeHelper
- [ ] IRuleDataHandler i RuleDataHandler
- [ ] IRuleService i RuleService (z walidacją)
- [ ] Endpoint.cs z mapowaniem API

### Faza 3: Logika TimeSlots
- [ ] Walidacja granulacji czasu (15 min)
- [ ] Walidacja nakładających się slotów
- [ ] CRUD operacje na TimeSlots
- [ ] Algorytm dopasowania aktywnej reguły

### Faza 4: Testy API (WYMAGANE przed showcase)
> **WAŻNE:** Testy endpointów API muszą być napisane i przechodzić PRZED uruchomieniem showcase app.

- [ ] Infrastruktura testowa (Container, Factory, Seeder)
- [ ] Testy CRUD reguł (Create, Read, Update, Delete)
- [ ] Testy walidacji (brak nazwy, błędna granulacja czasu, nakładające się sloty)
- [ ] Testy algorytmu dopasowania (/check endpoint)
- [ ] Testy autoryzacji (role)
- [ ] **Wszystkie testy PASS** ✓

### Faza 5: Integracja
- [ ] Dodanie do solution
- [ ] Konfiguracja Gateway (YARP routing)
- [ ] SwaggerAggregator
- [ ] Seedowanie danych przykładowych

---

## 16. Uwagi końcowe

1. **Testy API PRZED showcase** - wszystkie testy endpointów muszą przechodzić przed uruchomieniem showcase app
2. **Granulacja 15 minut** - umożliwia elastyczność przy zachowaniu prostoty (96 slotów na dzień)
3. **Priority** - niższa wartość = wyższy priorytet (intuicyjne dla admina)
4. **Soft delete** - zachowanie historii reguł
5. **Email notification** - przygotowana struktura, implementacja wysyłki w przyszłości
6. **Endpoint /check** - optymalizowany pod kątem wydajności (używany przy każdym połączeniu)
