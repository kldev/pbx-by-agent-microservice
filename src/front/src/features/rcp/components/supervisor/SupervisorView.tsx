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
import { useGetSupervisorPeriodEntries } from "../../../../api/rcp/endpoints/rcp-supervisor/rcp-supervisor";
import { timeEntryStatusLabels } from "../../../../constants/enumLabels";
import {
	HintHighlight,
	HintList,
	HintListItem,
	HintSection,
	HintWarning,
	SectionHint,
} from "../shared";
import EntriesTable from "./EntriesTable";
import EntryDetailPanel from "./EntryDetailPanel";

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

interface SupervisorViewProps {
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

const SupervisorView: React.FC<SupervisorViewProps> = ({ year, month }) => {
	const styles = useStyles();
	const [statusFilter, setStatusFilter] = useState<string>("all");
	const [selectedGid, setSelectedGid] = useState<string | null>(null);

	const getEntriesMutation = useGetSupervisorPeriodEntries();
	const { data, isPending: isLoading, isError } = getEntriesMutation;

	// Fetch data when year/month changes
	useEffect(() => {
		getEntriesMutation.mutate({ data: { year, month } });
	}, [year, month]);

	const refetch = () => {
		getEntriesMutation.mutate({ data: { year, month } });
	};

	const entries = data?.status === 200 ? (data.data.entries ?? []) : [];

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

	const handleStatusChange = () => {
		refetch();
		setSelectedGid(null);
	};

	if (isLoading) {
		return (
			<div className={styles.loading}>
				<Spinner label="Ładowanie wpisów..." />
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
			<SectionHint title="Jak zatwierdzać godziny?" storageKey="rcp_supervisor">
				<HintSection title="Proces zatwierdzania">
					<HintList>
						<HintListItem>
							Pracownik wysyła swoje godziny (status:{" "}
							<HintHighlight>Wysłane</HintHighlight>)
						</HintListItem>
						<HintListItem>
							Kliknij w wiersz, aby zobaczyć szczegóły i podjąć decyzję
						</HintListItem>
						<HintListItem>
							<HintHighlight>Zatwierdź</HintHighlight> - godziny są poprawne
						</HintListItem>
						<HintListItem>
							<HintHighlight>Do poprawy</HintHighlight> - pracownik musi
							skorygować
						</HintListItem>
						<HintListItem>
							<HintHighlight>Do rozliczenia</HintHighlight> - przekaż
							zatwierdzone do kadr
						</HintListItem>
					</HintList>
				</HintSection>

				<HintSection title="Co sprawdzić przed zatwierdzeniem?">
					<HintList>
						<HintListItem>
							Czy suma godzin zgadza się z harmonogramem?
						</HintListItem>
						<HintListItem>
							Czy godziny nadliczbowe były uzgodnione?
						</HintListItem>
						<HintListItem>
							Czy wszystkie dni robocze są wypełnione?
						</HintListItem>
					</HintList>
				</HintSection>

				<HintSection>
					<HintWarning>Ważne:</HintWarning> Przy zwracaniu do poprawy ZAWSZE
					dodaj komentarz wyjaśniający co jest nieprawidłowe.
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

			<EntriesTable entries={filteredEntries} onEntryClick={handleEntryClick} />

			{selectedGid && (
				<EntryDetailPanel
					gid={selectedGid}
					onClose={handlePanelClose}
					onStatusChange={handleStatusChange}
					mode="supervisor"
				/>
			)}
		</div>
	);
};

export default SupervisorView;
