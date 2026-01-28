import {
	Dropdown,
	MessageBar,
	MessageBarBody,
	makeStyles,
	Option,
	Spinner,
	tokens,
} from "@fluentui/react-components";
import type React from "react";
import { useEffect, useState } from "react";
import { useGetSupervisorMonitorPeriod } from "../../../../api/rcp/endpoints/rcp-supervisor/rcp-supervisor";
import { timeEntryStatusLabels } from "../../../../constants/enumLabels";
import {
	HintHighlight,
	HintList,
	HintListItem,
	HintSection,
	SectionHint,
} from "../shared";
import MonitoringDetailPanel from "./MonitoringDetailPanel";
import MonitoringTable from "./MonitoringTable";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalM,
	},
	filters: {
		display: "flex",
		gap: tokens.spacingHorizontalM,
		alignItems: "center",
	},
	filterDropdown: {
		minWidth: "200px",
	},
	loading: {
		display: "flex",
		justifyContent: "center",
		alignItems: "center",
		padding: tokens.spacingVerticalXXL,
	},
});

interface MonitoringViewProps {
	year: number;
	month: number;
}

const STATUS_OPTIONS = [
	{ value: "all", label: "Wszystkie statusy" },
	...Object.entries(timeEntryStatusLabels).map(([value, label]) => ({
		value,
		label,
	})),
];

const MonitoringView: React.FC<MonitoringViewProps> = ({ year, month }) => {
	const styles = useStyles();
	const [statusFilter, setStatusFilter] = useState<string>("all");
	const [selectedGid, setSelectedGid] = useState<string | null>(null);

	const getEntriesMutation = useGetSupervisorMonitorPeriod();
	const { data, isPending: isLoading, isError } = getEntriesMutation;

	useEffect(() => {
		getEntriesMutation.mutate({ data: { year, month } });
	}, [year, month]);

	const entries = data?.status === 200 ? (data.data.entries ?? []) : [];
	const workingDaysInMonth =
		data?.status === 200 ? (data.data.workingDaysInMonth ?? 20) : 20;

	const filteredEntries =
		statusFilter === "all"
			? entries
			: entries.filter((e) => e.status === statusFilter);

	const handleEntryClick = (gid: string) => {
		setSelectedGid(gid);
	};

	const handlePanelClose = () => {
		setSelectedGid(null);
	};

	if (isLoading) {
		return (
			<div className={styles.loading}>
				<Spinner label="Ładowanie podwładnych..." />
			</div>
		);
	}

	if (isError) {
		return (
			<MessageBar intent="error">
				<MessageBarBody>
					Wystąpił błąd podczas ładowania danych. Spróbuj ponownie.
				</MessageBarBody>
			</MessageBar>
		);
	}

	return (
		<div className={styles.container}>
			<SectionHint title="Jak działa monitoring?" storageKey="rcp_monitoring">
				<HintSection title="Cel monitoringu">
					<HintList>
						<HintListItem>
							Śledź postęp podwładnych <HintHighlight>przed</HintHighlight>{" "}
							wysłaniem godzin
						</HintListItem>
						<HintListItem>
							Zobacz ile dni zostało wypełnionych w porównaniu do dni roboczych
						</HintListItem>
						<HintListItem>Sprawdź datę ostatniego wpisu</HintListItem>
					</HintList>
				</HintSection>

				<HintSection title="Statusy">
					<HintList>
						<HintListItem>
							<HintHighlight>Roboczy</HintHighlight> - pracownik jeszcze
							wprowadza dane
						</HintListItem>
						<HintListItem>
							<HintHighlight>Wysłane</HintHighlight> - czeka na zatwierdzenie
							(przejdź do "Zatwierdzanie")
						</HintListItem>
						<HintListItem>
							<HintHighlight>Zatwierdzone</HintHighlight> - wpis zatwierdzony
						</HintListItem>
					</HintList>
				</HintSection>
			</SectionHint>

			<div className={styles.filters}>
				<Dropdown
					className={styles.filterDropdown}
					value={
						STATUS_OPTIONS.find((o) => o.value === statusFilter)?.label ??
						"Wszystkie statusy"
					}
					selectedOptions={[statusFilter]}
					onOptionSelect={(_, data) =>
						setStatusFilter(data.optionValue ?? "all")
					}
				>
					{STATUS_OPTIONS.map((opt) => (
						<Option key={opt.value} value={opt.value}>
							{opt.label}
						</Option>
					))}
				</Dropdown>
			</div>

			<MonitoringTable
				entries={filteredEntries}
				workingDaysInMonth={workingDaysInMonth}
				onEntryClick={handleEntryClick}
			/>

			{selectedGid && (
				<MonitoringDetailPanel gid={selectedGid} onClose={handlePanelClose} />
			)}
		</div>
	);
};

export default MonitoringView;
