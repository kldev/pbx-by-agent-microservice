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
import { useGetPayrollPeriodEntries } from "../../../../api/rcp/endpoints/rcp-payroll/rcp-payroll";
import { RcpTimeEntryStatus } from "../../../../api/rcp/models";
import { timeEntryStatusLabels } from "../../../../constants/enumLabels";
import {
	HintHighlight,
	HintList,
	HintListItem,
	HintSection,
	HintWarning,
	SectionHint,
} from "../shared";
import { EntriesTable, EntryDetailPanel } from "../supervisor";
import ExportButton from "./ExportButton";

const useStyles = makeStyles({
	container: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalM,
	},
	toolbar: {
		display: "flex",
		justifyContent: "space-between",
		alignItems: "center",
		gap: tokens.spacingHorizontalM,
		flexWrap: "wrap",
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

interface PayrollViewProps {
	year: number;
	month: number;
}

const STATUS_OPTIONS = [
	{ value: "all", label: "Wszystkie statusy" },
	{
		value: RcpTimeEntryStatus.Settlement,
		label: timeEntryStatusLabels[RcpTimeEntryStatus.Settlement],
	},
	{
		value: RcpTimeEntryStatus.Approved,
		label: timeEntryStatusLabels[RcpTimeEntryStatus.Approved],
	},
];

const PayrollView: React.FC<PayrollViewProps> = ({ year, month }) => {
	const styles = useStyles();
	const [statusFilter, setStatusFilter] = useState<string>("all");
	const [selectedGid, setSelectedGid] = useState<string | null>(null);

	const getEntriesMutation = useGetPayrollPeriodEntries();
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
			<SectionHint title="Jak rozliczać czas pracy?" storageKey="rcp_payroll">
				<HintSection title="Proces rozliczenia">
					<HintList>
						<HintListItem>
							Przełożeni zatwierdzają godziny i przekazują do rozliczenia
						</HintListItem>
						<HintListItem>
							Wpisy ze statusem <HintHighlight>Rozliczenie</HintHighlight> są
							gotowe do eksportu
						</HintListItem>
						<HintListItem>
							Kliknij w wiersz, aby zobaczyć szczegóły wpisu
						</HintListItem>
					</HintList>
				</HintSection>

				<HintSection title="Eksport do Excel">
					Kliknij "Eksportuj do Excel" aby pobrać plik z danymi do systemu
					płacowego. Plik zawiera: imię i nazwisko, sumę godzin, notatki.
				</HintSection>

				<HintSection>
					<HintWarning>Ważne:</HintWarning> Eksportuj dopiero gdy WSZYSCY
					pracownicy mają status "Rozliczenie" za dany miesiąc.
				</HintSection>
			</SectionHint>

			<div className={styles.toolbar}>
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
				<ExportButton year={year} month={month} />
			</div>

			<EntriesTable entries={filteredEntries} onEntryClick={handleEntryClick} />

			{selectedGid && (
				<EntryDetailPanel
					gid={selectedGid}
					onClose={handlePanelClose}
					onStatusChange={handleStatusChange}
					mode="payroll"
				/>
			)}
		</div>
	);
};

export default PayrollView;
