import {
	makeStyles,
	mergeClasses,
	Table,
	TableBody,
	TableCell,
	TableHeader,
	TableHeaderCell,
	TableRow,
	Text,
	tokens,
} from "@fluentui/react-components";
import type React from "react";
import type { MonthlyEntrySummaryResponse } from "../../../../api/rcp/models";
import { toTimeFormat } from "../../utils";
import RcpStatusBadge from "../RcpStatusBadge";

const useStyles = makeStyles({
	table: {
		width: "100%",
		backgroundColor: tokens.colorNeutralBackground1,
		tableLayout: "fixed",
	},
	headerRow: {
		backgroundColor: tokens.colorNeutralBackground4,
	},
	headerCell: {
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorNeutralForeground1,
	},
	row: {
		cursor: "pointer",
		":hover": {
			backgroundColor: tokens.colorNeutralBackground1Hover,
		},
	},
	rowEven: {
		cursor: "pointer",
		backgroundColor: tokens.colorNeutralBackground2,
		":hover": {
			backgroundColor: tokens.colorNeutralBackground2Hover,
		},
	},
	employeeCell: {
		fontWeight: tokens.fontWeightSemibold,
		width: "40%",
	},
	hoursCell: {
		fontWeight: tokens.fontWeightSemibold,
		color: tokens.colorBrandForeground1,
		width: "15%",
	},
	statusCell: {
		width: "25%",
	},
	dateCell: {
		width: "20%",
	},
	emptyState: {
		textAlign: "center",
		padding: tokens.spacingVerticalXXL,
		color: tokens.colorNeutralForeground3,
	},
});

interface EntriesTableProps {
	entries: MonthlyEntrySummaryResponse[];
	onEntryClick: (gid: string) => void;
}

const EntriesTable: React.FC<EntriesTableProps> = ({
	entries,
	onEntryClick,
}) => {
	const styles = useStyles();

	if (entries.length === 0) {
		return (
			<div className={styles.emptyState}>
				<Text>Brak wpisów do wyświetlenia</Text>
			</div>
		);
	}

	return (
		<Table className={styles.table} aria-label="Lista wpisów RCP">
			<TableHeader>
				<TableRow className={styles.headerRow}>
					<TableHeaderCell
						className={mergeClasses(styles.headerCell, styles.employeeCell)}
					>
						Pracownik
					</TableHeaderCell>
					<TableHeaderCell
						className={mergeClasses(styles.headerCell, styles.hoursCell)}
					>
						Suma godzin
					</TableHeaderCell>
					<TableHeaderCell
						className={mergeClasses(styles.headerCell, styles.statusCell)}
					>
						Status
					</TableHeaderCell>
					<TableHeaderCell
						className={mergeClasses(styles.headerCell, styles.dateCell)}
					>
						Wysłano
					</TableHeaderCell>
				</TableRow>
			</TableHeader>
			<TableBody>
				{entries.map((entry, index) => (
					<TableRow
						key={entry.gid}
						className={index % 2 === 1 ? styles.rowEven : styles.row}
						onClick={() => entry.gid && onEntryClick(entry.gid)}
					>
						<TableCell className={styles.employeeCell}>
							{entry.employeeFullName ?? "—"}
						</TableCell>
						<TableCell className={styles.hoursCell}>
							{toTimeFormat(entry.totalMinutes ?? 0)}
						</TableCell>
						<TableCell className={styles.statusCell}>
							<RcpStatusBadge status={entry.status} />
						</TableCell>
						<TableCell className={styles.dateCell}>
							{entry.submittedAt
								? new Date(entry.submittedAt).toLocaleDateString("pl-PL")
								: "—"}
						</TableCell>
					</TableRow>
				))}
			</TableBody>
		</Table>
	);
};

export default EntriesTable;
