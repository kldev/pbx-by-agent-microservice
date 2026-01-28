# Plan: Frontend - Modul Answering Rules

## 1. Zakres

Dodanie modulu `answering-rules` do frontendu (`src/front/src/features/answering-rules/`) z pelnym CRUD regul odpowiadania na polaczenia. Konto SIP picker - **fake** (random GID + random login SIP, bez wywolania API).

API hooks juz wygenerowane przez Orval w `src/front/src/api/answerrule/`.

---

## 2. Struktura plikow

```
src/front/src/features/answering-rules/
├── pages/
│   ├── AnsweringRuleListPage.tsx     # Lista regul z filtrami i paginacja
│   ├── AnsweringRuleCreatePage.tsx   # Tworzenie nowej reguly
│   ├── AnsweringRuleDetailPage.tsx   # Szczegoly reguly (read-only)
│   ├── AnsweringRuleEditPage.tsx     # Edycja reguly
│   └── index.ts                     # Eksporty stron
├── components/
│   ├── AnsweringRuleTable.tsx        # Tabela regul (desktop) + karty (mobile)
│   ├── AnsweringRuleForm.tsx         # Formularz tworzenia/edycji reguly
│   ├── TimeSlotsEditor.tsx           # Edytor przedzialow czasowych (tabela z dodawaniem/usuwaniem)
│   ├── ActionTypeBadge.tsx           # Badge wyswietlajacy typ akcji (Voicemail/Redirect/...)
│   ├── SipAccountPicker.tsx          # FAKE picker - generuje random GID + login
│   └── index.ts                     # Eksporty komponentow
└── index.ts                         # Glowny eksport modulu
```

---

## 3. Routing

### 3.1 Nowe route'y w `src/front/src/routes/routes.ts`

```typescript
// Answering Rules
ANSWERING_RULES_LIST: "/answering-rules",
ANSWERING_RULES_CREATE: "/answering-rules/new",
ANSWERING_RULES_DETAIL: "/answering-rules/:gid",
ANSWERING_RULES_EDIT: "/answering-rules/:gid/edit",
```

Plus helper:
```typescript
export function getAnsweringRuleDetailPath(gid: string): string {
    return `/answering-rules/${gid}`;
}
```

### 3.2 Router (`router.tsx`)

- Lazy load wszystkich 4 stron
- Nowy `RoleGuard` w sekcji PBX (obok CDR, **nie** w sekcji Systemowe)
- `allowedRoles={PBX_ROLES}` gdzie `PBX_ROLES = [AppRole.Root, AppRole.Admin, AppRole.Ops]`
- Ops ma pelny dostep do answering rules (zgodnie z backendem: AdminRoles = Root, Ops, Admin)

### 3.3 Sidebar (`Sidebar.tsx`)

- Dodac NavItem w grupie "PBX" (obok CDR)
- Ikona: `CallForwardFilled` / `CallForwardRegular` z `@fluentui/react-icons`
- Label: "Reguly odpowiadania"
- Widoczny dla wszystkich (nie tylko admin) - bo AllRoles moze przegladac

---

## 4. Strony - szczegoly implementacji

### 4.1 AnsweringRuleListPage

**Wzorowane na:** `TeamListPage`

**Elementy:**
- PageHeader: tytul "Reguly odpowiadania", breadcrumbs, przycisk "Nowa regula"
- Wyszukiwanie z debounce (300ms)
- Filtr: status (Wszystkie / Aktywne / Nieaktywne) jako taby
- Filtr: typ akcji (dropdown - Voicemail, Redirect, RedirectToGroup, DisconnectWithVoicemessage)
- Paginacja (prev/next)
- Tabela `AnsweringRuleTable` z akcjami: Szczegoly, Edytuj, Wlacz/Wylacz, Usun

**API:** `useGetRulesList()` mutation z `AnsweringRuleListFilter`

### 4.2 AnsweringRuleCreatePage

**Elementy:**
- PageHeader: "Nowa regula", breadcrumbs
- Formularz `AnsweringRuleForm` (isEditMode=false)
- Submit -> `useCreateRule()` -> navigate do detail page

### 4.3 AnsweringRuleDetailPage

**Elementy:**
- PageHeader z breadcrumbs i akcjami (Edytuj, Wlacz/Wylacz, Usun)
- Sekcja: Informacje ogolne (nazwa, opis, priorytet, status, typ akcji, cel akcji)
- Sekcja: Konto SIP (sipAccountGid)
- Sekcja: Notyfikacje (email, czy wlaczone)
- Sekcja: Przedzialy czasowe - tabela z kolumnami: Dzien, Od, Do, Caly dzien
- Info systemowe (GID, data utworzenia)

**API:** `useGetRuleByGid(gid)` query

### 4.4 AnsweringRuleEditPage

**Elementy:**
- PageHeader: "Edycja reguly", breadcrumbs
- Fetch danych -> `AnsweringRuleForm` z initialData (isEditMode=true)
- Submit -> `useUpdateRule()` -> navigate do detail page

---

## 5. Komponenty - szczegoly

### 5.1 AnsweringRuleTable

**Wzorowane na:** `TeamTable`

**Kolumny desktop:**
| Kolumna | Pole | Uwagi |
|---------|------|-------|
| Nazwa | name | truncate z tooltip |
| Typ akcji | actionType | ActionTypeBadge |
| Cel | actionTarget | "-" jesli brak |
| Priorytet | priority | Badge numeryczny |
| Sloty | timeSlotsCount | np. "5 slotow" |
| Status | isEnabled | Badge Aktywna/Nieaktywna |
| Akcje | - | Menu: Szczegoly, Edytuj, Wlacz/Wylacz, Usun |

**Mobile:** karty z glownymi info

### 5.2 AnsweringRuleForm

**React Hook Form + Controller z Fluent UI**

**Pola formularza:**

| Pole | Typ | Walidacja | Uwagi |
|------|-----|-----------|-------|
| sipAccountGid | SipAccountPicker (Dropdown) | required | FAKE picker |
| name | Input | required, max 100 | |
| description | Textarea | max 500 | opcjonalne |
| priority | SpinButton/Input | required, >=0 | domyslnie 100 |
| isEnabled | Checkbox | - | domyslnie true, tylko w edycji |
| actionType | Dropdown | required | enum AnsweringRuleAction |
| actionTarget | Input | required gdy Redirect/RedirectToGroup | warunkowe wyswietlanie |
| voicemailBoxGid | Input | required gdy Voicemail | warunkowe wyswietlanie |
| voiceMessageGid | Input | required gdy DisconnectWithVoicemessage | warunkowe wyswietlanie |
| sendEmailNotification | Checkbox | - | |
| notificationEmail | Input | email format gdy wypelnione | widoczne gdy sendEmailNotification=true |
| timeSlots | TimeSlotsEditor | min 1 slot | osobny komponent |

**Logika warunkowa `actionType`:**
- `Voicemail` -> pokaz `voicemailBoxGid`
- `Redirect` -> pokaz `actionTarget` (label: "Numer docelowy")
- `RedirectToGroup` -> pokaz `actionTarget` (label: "GID grupy")
- `DisconnectWithVoicemessage` -> pokaz `voiceMessageGid`

### 5.3 TimeSlotsEditor

**Komponent do zarzadzania przedzialami czasowymi w formularzu.**

**Elementy:**
- Tabela istniejacych slotow z kolumnami: Dzien tygodnia, Od, Do, Caly dzien, Usun
- Przycisk "Dodaj przedzial" otwiera wiersz do edycji
- Dzien tygodnia: Dropdown (Poniedzialek-Niedziela)
- Od/Do: Dropdown z opcjami co 15 min (00:00, 00:15, ..., 23:45)
- Caly dzien: Checkbox (gdy zaznaczony - ukrywa Od/Do)
- Walidacja: EndTime > StartTime, brak nakladajacych sie slotow w tym samym dniu

**Dane przechowywane w formularzu jako `TimeSlotDto[]`**

### 5.4 ActionTypeBadge

**Mapowanie ActionType na czytelne etykiety:**
- `Voicemail` -> "Poczta glosowa" (ikona Voicemail)
- `Redirect` -> "Przekierowanie" (ikona CallForward)
- `RedirectToGroup` -> "Grupa" (ikona People)
- `DisconnectWithVoicemessage` -> "Komunikat" (ikona Speaker)

Badge z kolorem i ikona.

### 5.5 SipAccountPicker (FAKE)

**Fake picker generujacy losowe dane konta SIP.**

Dropdown z lista 5-10 fejkowych kont SIP:
```typescript
const FAKE_SIP_ACCOUNTS = [
    { gid: randomGid(), label: "sip_user_101@pbx.local" },
    { gid: randomGid(), label: "sip_user_102@pbx.local" },
    { gid: randomGid(), label: "sip_reception@pbx.local" },
    { gid: randomGid(), label: "sip_support@pbx.local" },
    { gid: randomGid(), label: "sip_sales@pbx.local" },
];

function randomGid(): string {
    return crypto.randomUUID().replace(/-/g, "");
}
```

Komponent renderuje Fluent UI `Dropdown` z tymi opcjami. Zwraca wybrany `gid` do formularza.

---

## 6. Mapowanie etykiet polskich

```typescript
const ACTION_TYPE_LABELS: Record<AnsweringRuleAction, string> = {
    Voicemail: "Poczta glosowa",
    Redirect: "Przekierowanie",
    RedirectToGroup: "Przekierowanie do grupy",
    DisconnectWithVoicemessage: "Rozlaczenie z komunikatem",
};

const DAY_OF_WEEK_LABELS: Record<DayOfWeek, string> = {
    Monday: "Poniedzialek",
    Tuesday: "Wtorek",
    Wednesday: "Sroda",
    Thursday: "Czwartek",
    Friday: "Piatek",
    Saturday: "Sobota",
    Sunday: "Niedziela",
};
```

---

## 7. Modyfikacje istniejacych plikow

| Plik | Zmiana |
|------|--------|
| `src/front/src/routes/routes.ts` | Dodanie 4 route'ow answering-rules + helper |
| `src/front/src/routes/router.tsx` | Lazy load stron + trasy w RoleGuard |
| `src/front/src/components/layout/Sidebar.tsx` | NavItem w grupie PBX |

---

## 8. Kolejnosc implementacji

1. **Routes & navigation** - routes.ts, router.tsx, Sidebar.tsx
2. **Typy i helpery** - etykiety akcji, dni tygodnia, SipAccountPicker
3. **AnsweringRuleListPage** + **AnsweringRuleTable** + **ActionTypeBadge**
4. **AnsweringRuleDetailPage**
5. **TimeSlotsEditor**
6. **AnsweringRuleForm**
7. **AnsweringRuleCreatePage**
8. **AnsweringRuleEditPage**
9. **Eksporty** (index.ts na kazdym poziomie)
10. **Build & test** - `yarn build` w src/front

---

## 9. Uwagi

- Nie tworzymy nowych API hooks - korzystamy z istniejacych wygenerowanych przez Orval
- UI w jezyku polskim (zgodnie z reszta aplikacji)
- Responsive: tabela desktop + karty mobile (breakpoint 768px)
- Wzorzec jak w `features/teams/` - PageHeader, ConfirmDialog, LoadingSpinner, ErrorMessage, EmptyState
- Formularz: React Hook Form + Controller + Fluent UI Field
