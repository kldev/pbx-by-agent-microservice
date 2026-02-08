export const ROUTES = {
	LOGIN: "/login",
	DASHBOARD: "/",
	// Teams
	TEAMS_LIST: "/teams",
	TEAMS_CREATE: "/teams/new",
	TEAMS_DETAIL: "/teams/:gid",
	TEAMS_EDIT: "/teams/:gid/edit",
	// System Users (Admin)
	SYSTEM_USERS_LIST: "/system-users",
	SYSTEM_USERS_CREATE: "/system-users/new",
	SYSTEM_USERS_DETAIL: "/system-users/:gid",
	SYSTEM_USERS_EDIT: "/system-users/:gid/edit",
	// CDR
	CDR_LIST: "/cdr",
	CDR_DETAIL: "/cdr/:gid",
	// Answering Rules
	ANSWERING_RULES_LIST: "/answering-rules",
	ANSWERING_RULES_CREATE: "/answering-rules/new",
	ANSWERING_RULES_DETAIL: "/answering-rules/:gid",
	ANSWERING_RULES_EDIT: "/answering-rules/:gid/edit",
	// RCP
	RCP: "/rcp",
	// Costs
	COSTS_LIST: "/costs",
	// Dictionary Config
	DICTIONARY_CONFIG: "/settings/dictionaries",
} as const;

export function getTeamDetailPath(gid: string): string {
	return `/teams/${gid}`;
}

export function getCdrDetailPath(gid: string): string {
	return `/cdr/${gid}`;
}

export function getAnsweringRuleDetailPath(gid: string): string {
	return `/answering-rules/${gid}`;
}
