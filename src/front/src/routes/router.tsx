import { lazy } from "react";
import { createBrowserRouter } from "react-router";
// Eager load tylko auth pages (pierwsze co user widzi)
import { LoginPage } from "../features/auth/pages";
import ProtectedRoute from "./ProtectedRoute";
import RoleGuard from "./RoleGuard";
import { ROUTES } from "./routes";
import { AppRole } from "../api/identity/models";
import { DashboardPage } from "../features/dashboard";

// Role definitions matching backend AppRole enum
const ADMIN_ROLES = [AppRole.Root, AppRole.Admin];
const PBX_ROLES = [AppRole.Root, AppRole.Admin, AppRole.Ops];

// System Users
const SystemUserListPage = lazy(() =>
	import("../features/system-users/pages").then((m) => ({
		default: m.SystemUserListPage,
	})),
);
const SystemUserDetailPage = lazy(() =>
	import("../features/system-users/pages").then((m) => ({
		default: m.SystemUserDetailPage,
	})),
);
const SystemUserCreatePage = lazy(() =>
	import("../features/system-users/pages").then((m) => ({
		default: m.SystemUserCreatePage,
	})),
);
const SystemUserEditPage = lazy(() =>
	import("../features/system-users/pages").then((m) => ({
		default: m.SystemUserEditPage,
	})),
);

// Teams
const TeamListPage = lazy(() =>
	import("../features/teams/pages").then((m) => ({
		default: m.TeamListPage,
	})),
);
const TeamCreatePage = lazy(() =>
	import("../features/teams/pages").then((m) => ({
		default: m.TeamCreatePage,
	})),
);
const TeamDetailPage = lazy(() =>
	import("../features/teams/pages").then((m) => ({
		default: m.TeamDetailPage,
	})),
);
const TeamEditPage = lazy(() =>
	import("../features/teams/pages").then((m) => ({
		default: m.TeamEditPage,
	})),
);

// CDR
const CdrListPage = lazy(() =>
	import("../features/cdr/pages").then((m) => ({
		default: m.CdrListPage,
	})),
);
const CdrDetailPage = lazy(() =>
	import("../features/cdr/pages").then((m) => ({
		default: m.CdrDetailPage,
	})),
);

// RCP
const RcpPage = lazy(() => import("../features/rcp/pages/RcpPage"));

// Costs
const CostsListPage = lazy(() =>
	import("../features/costs/pages").then((m) => ({
		default: m.CostsListPage,
	})),
);

// Answering Rules
const AnsweringRuleListPage = lazy(() =>
	import("../features/answering-rules/pages").then((m) => ({
		default: m.AnsweringRuleListPage,
	})),
);
const AnsweringRuleCreatePage = lazy(() =>
	import("../features/answering-rules/pages").then((m) => ({
		default: m.AnsweringRuleCreatePage,
	})),
);
const AnsweringRuleDetailPage = lazy(() =>
	import("../features/answering-rules/pages").then((m) => ({
		default: m.AnsweringRuleDetailPage,
	})),
);
const AnsweringRuleEditPage = lazy(() =>
	import("../features/answering-rules/pages").then((m) => ({
		default: m.AnsweringRuleEditPage,
	})),
);

export const router = createBrowserRouter([
	{
		path: ROUTES.LOGIN,
		element: <LoginPage />,
	},
	{
		element: <ProtectedRoute />,
		children: [
			{
				path: ROUTES.DASHBOARD,
				element: <DashboardPage />,
			},
			// === SYSTEMOWE - tylko Admin/Manager ===
			{
				element: <RoleGuard allowedRoles={ADMIN_ROLES} />,
				children: [
					// Teams
					{
						path: ROUTES.TEAMS_LIST,
						element: <TeamListPage />,
					},
					{
						path: ROUTES.TEAMS_CREATE,
						element: <TeamCreatePage />,
					},
					{
						path: ROUTES.TEAMS_DETAIL,
						element: <TeamDetailPage />,
					},
					{
						path: ROUTES.TEAMS_EDIT,
						element: <TeamEditPage />,
					},
					// System Users
					{
						path: ROUTES.SYSTEM_USERS_LIST,
						element: <SystemUserListPage />,
					},
					{
						path: ROUTES.SYSTEM_USERS_CREATE,
						element: <SystemUserCreatePage />,
					},
					{
						path: ROUTES.SYSTEM_USERS_DETAIL,
						element: <SystemUserDetailPage />,
					},
					{
						path: ROUTES.SYSTEM_USERS_EDIT,
						element: <SystemUserEditPage />,
					},
					// CDR
					{
						path: ROUTES.CDR_LIST,
						element: <CdrListPage />,
					},
					{
						path: ROUTES.CDR_DETAIL,
						element: <CdrDetailPage />,
					},
					// Costs
					{
						path: ROUTES.COSTS_LIST,
						element: <CostsListPage />,
					},
				],
			},
			// === PBX - Root/Admin/Ops ===
			{
				element: <RoleGuard allowedRoles={PBX_ROLES} />,
				children: [
					// Answering Rules
					{
						path: ROUTES.ANSWERING_RULES_LIST,
						element: <AnsweringRuleListPage />,
					},
					{
						path: ROUTES.ANSWERING_RULES_CREATE,
						element: <AnsweringRuleCreatePage />,
					},
					{
						path: ROUTES.ANSWERING_RULES_DETAIL,
						element: <AnsweringRuleDetailPage />,
					},
					{
						path: ROUTES.ANSWERING_RULES_EDIT,
						element: <AnsweringRuleEditPage />,
					},
				],
			},
			// RCP - dla wszystkich zalogowanych (kontrola dostępu w komponencie)
			{
				path: ROUTES.RCP,
				element: <RcpPage />,
			},
		],
	},
]);
