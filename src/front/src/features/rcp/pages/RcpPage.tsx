import {
	Button,
	MessageBar,
	MessageBarBody,
	MessageBarTitle,
	makeStyles,
	Tab,
	TabList,
	tokens,
} from "@fluentui/react-components";
import { LockClosedRegular } from "@fluentui/react-icons";
import type React from "react";
import { useState } from "react";
import { useNavigate } from "react-router";

import { PageHeader } from "../../../components/common";
import { useAuthStore } from "../../../stores";
import { MonthNavigator } from "../components";
import { MonitoringView } from "../components/monitoring";
import { MyMonthView } from "../components/my";
import { PayrollView } from "../components/payroll";
import { SupervisorView } from "../components/supervisor";
import { useMonthNavigation } from "../hooks";
import { AppRole } from "../../../api/identity/models";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		height: "100%",
		padding: tokens.spacingHorizontalL,
		gap: tokens.spacingVerticalL,
	},
	header: {
		display: "flex",
		justifyContent: "space-between",
		alignItems: "center",
		flexWrap: "wrap",
		gap: tokens.spacingHorizontalM,
	},
	tabContent: {
		flex: 1,
		overflow: "auto",
	},
	accessDenied: {
		display: "flex",
		flexDirection: "column",
		alignItems: "center",
		justifyContent: "center",
		height: "100%",
		gap: tokens.spacingVerticalL,
		padding: tokens.spacingHorizontalXXL,
	},
	accessDeniedIcon: {
		fontSize: "64px",
		color: tokens.colorNeutralForeground3,
	},
	accessDeniedMessage: {
		maxWidth: "500px",
	},
});

type RcpTab = "my" | "monitoring" | "supervisor" | "payroll";

const RcpPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const { year, month, goToPrevious, goToNext, goToToday } =
		useMonthNavigation();
	const [activeTab, setActiveTab] = useState<RcpTab>("my");
	const { user } = useAuthStore();

	const userRoles = user?.roles ?? [];
	const hasRole = (role: AppRole) => userRoles.includes(role);
	// W mikroserwisach każdy zalogowany użytkownik ma dostęp do RCP
	// ResponsiblePerson może zgłaszać godziny, Supervisor monitoruje/zatwierdza
	const hasRcpAccess =
		hasRole(AppRole.Ops) || hasRole(AppRole.Admin) || hasRole(AppRole.Root);

	const isSupervisor = hasRole(AppRole.Admin) || hasRole(AppRole.Root);
	const isPayroll = hasRole(AppRole.Admin) || hasRole(AppRole.Root);

	const handleTabChange = (_: unknown, data: { value: unknown }) => {
		setActiveTab(data.value as RcpTab);
	};

	// Brak dostępu do modułu RCP
	if (!hasRcpAccess) {
		return (
			<div className={styles.container}>
				<PageHeader
					title="Czas pracy (RCP)"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{ label: "Czas pracy" },
					]}
				/>
				<div className={styles.accessDenied}>
					<LockClosedRegular className={styles.accessDeniedIcon} />
					<MessageBar intent="warning" className={styles.accessDeniedMessage}>
						<MessageBarBody>
							<MessageBarTitle>Brak dostępu do modułu RCP</MessageBarTitle>
							Nie masz uprawnień do korzystania z modułu Czas pracy. Aby uzyskać
							dostęp, skontaktuj się z administratorem lub działem HR.
						</MessageBarBody>
					</MessageBar>
					<Button appearance="primary" onClick={() => navigate("/")}>
						Wróć do strony głównej
					</Button>
				</div>
			</div>
		);
	}

	return (
		<div className={styles.container}>
			<div className={styles.header}>
				<PageHeader
					title="Czas pracy (RCP)"
					breadcrumbs={[
						{ label: "Dashboard", href: "/" },
						{ label: "Czas pracy" },
					]}
				/>
				<MonthNavigator
					year={year}
					month={month}
					onPrevious={goToPrevious}
					onNext={goToNext}
					onToday={goToToday}
				/>
			</div>

			<TabList selectedValue={activeTab} onTabSelect={handleTabChange}>
				<Tab value="my">Moje godziny</Tab>
				{isSupervisor && <Tab value="monitoring">Monitoring</Tab>}
				{isSupervisor && <Tab value="supervisor">Zatwierdzanie</Tab>}
				{isPayroll && <Tab value="payroll">Rozliczenie</Tab>}
			</TabList>

			<div className={styles.tabContent}>
				{activeTab === "my" && <MyMonthView year={year} month={month} />}
				{activeTab === "monitoring" && isSupervisor && (
					<MonitoringView year={year} month={month} />
				)}
				{activeTab === "supervisor" && isSupervisor && (
					<SupervisorView year={year} month={month} />
				)}
				{activeTab === "payroll" && isPayroll && (
					<PayrollView year={year} month={month} />
				)}
			</div>
		</div>
	);
};

export default RcpPage;
