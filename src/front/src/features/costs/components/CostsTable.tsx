import {
	Card,
	CardHeader,
	makeStyles,
	mergeClasses,
	Table,
	TableBody,
	TableCell,
	TableHeader,
	TableHeaderCell,
	TableRow,
	Text,
	Tooltip,
	tokens,
} from "@fluentui/react-components";
import {
	CalendarRegular,
	MoneyRegular,
	PersonRegular,
} from "@fluentui/react-icons";
import type React from "react";
import type { DocumentEntryResponse } from "../../../api/fincosts/models";
import { PaymentStatusBadge } from "./";

const useStyles = makeStyles({
	tableContainer: {
		display: "block",
		"@media (max-width: 960px)": {
			display: "none",
		},
	},
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
		":hover": {
			backgroundColor: tokens.colorNeutralBackground1Hover,
		},
	},
	rowEven: {
		backgroundColor: tokens.colorNeutralBackground2,
		":hover": {
			backgroundColor: tokens.colorNeutralBackground2Hover,
		},
	},
	docTypeCell: {
		width: "140px",
	},
	issuedByCell: {
		width: "180px",
	},
	issuedForCell: {
		width: "180px",
	},
	amountCell: {
		width: "120px",
		textAlign: "right",
	},
	currencyCell: {
		width: "80px",
	},
	vatCell: {
		width: "70px",
	},
	statusCell: {
		width: "110px",
	},
	dateCell: {
		width: "140px",
	},
	truncatedCell: {
		display: "block",
		overflow: "hidden",
		textOverflow: "ellipsis",
		whiteSpace: "nowrap",
	},
	cardsContainer: {
		display: "none",
		flexDirection: "column",
		gap: tokens.spacingVerticalM,
		"@media (max-width: 960px)": {
			display: "flex",
		},
	},
	card: {},
	cardContent: {
		display: "flex",
		flexDirection: "column",
		gap: tokens.spacingVerticalS,
		padding: tokens.spacingVerticalS,
	},
	cardRow: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
		color: tokens.colorNeutralForeground2,
		fontSize: tokens.fontSizeBase200,
	},
	cardRowIcon: {
		flexShrink: 0,
		color: tokens.colorNeutralForeground3,
	},
	cardRowText: {
		overflow: "hidden",
		textOverflow: "ellipsis",
		whiteSpace: "nowrap",
	},
});

export interface CostsTableProps {
	items: DocumentEntryResponse[];
}

const TruncatedCell: React.FC<{ value?: string | null }> = ({ value }) => {
	const styles = useStyles();
	const displayValue = value || "-";

	return (
		<Tooltip content={displayValue} relationship="description">
			<span className={styles.truncatedCell}>{displayValue}</span>
		</Tooltip>
	);
};

function formatDateTime(dateStr?: string): string {
	if (!dateStr) return "-";
	const date = new Date(dateStr);
	return date.toLocaleString("pl-PL", {
		day: "2-digit",
		month: "2-digit",
		year: "numeric",
		hour: "2-digit",
		minute: "2-digit",
	});
}

function formatAmount(amount?: number): string {
	if (amount === undefined || amount === null) return "-";
	return amount.toFixed(2);
}

function formatVatRate(rate?: number): string {
	if (rate === undefined || rate === null) return "-";
	return `${rate}%`;
}

const CostsTable: React.FC<CostsTableProps> = ({ items }) => {
	const styles = useStyles();

	const TableView = (
		<div className={styles.tableContainer}>
			<Table aria-label="Lista dokumentów kosztowych" className={styles.table}>
				<TableHeader>
					<TableRow className={styles.headerRow}>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.docTypeCell)}
						>
							Typ dokumentu
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.issuedByCell)}
						>
							Wystawca
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.issuedForCell)}
						>
							Odbiorca
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.amountCell)}
						>
							Kwota
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.currencyCell)}
						>
							Waluta
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.vatCell)}
						>
							VAT
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.statusCell)}
						>
							Status
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.dateCell)}
						>
							Data
						</TableHeaderCell>
					</TableRow>
				</TableHeader>
				<TableBody>
					{items.map((item, index) => (
						<TableRow
							key={item.gid}
							className={index % 2 === 1 ? styles.rowEven : styles.row}
						>
							<TableCell className={styles.docTypeCell}>
								<TruncatedCell value={item.documentTypeNamePL} />
							</TableCell>
							<TableCell className={styles.issuedByCell}>
								<TruncatedCell value={item.issuedBy} />
							</TableCell>
							<TableCell className={styles.issuedForCell}>
								<TruncatedCell value={item.issuedFor} />
							</TableCell>
							<TableCell className={styles.amountCell}>
								{formatAmount(item.totalAmount)}
							</TableCell>
							<TableCell className={styles.currencyCell}>
								{item.currencyNamePL || "-"}
							</TableCell>
							<TableCell className={styles.vatCell}>
								{formatVatRate(item.vatRate)}
							</TableCell>
							<TableCell className={styles.statusCell}>
								<PaymentStatusBadge wasPaid={item.wasPaid} />
							</TableCell>
							<TableCell className={styles.dateCell}>
								<TruncatedCell value={formatDateTime(item.createdAt)} />
							</TableCell>
						</TableRow>
					))}
				</TableBody>
			</Table>
		</div>
	);

	const CardView = (
		<div className={styles.cardsContainer}>
			{items.map((item) => (
				<Card key={item.gid} className={styles.card}>
					<CardHeader
						header={
							<Text weight="semibold">
								{item.issuedBy || "Nieznany"} → {item.issuedFor || "Nieznany"}
							</Text>
						}
						description={
							<div
								style={{
									display: "flex",
									alignItems: "center",
									gap: "8px",
									marginTop: "4px",
								}}
							>
								<PaymentStatusBadge wasPaid={item.wasPaid} />
							</div>
						}
					/>
					<div className={styles.cardContent}>
						<div className={styles.cardRow}>
							<MoneyRegular className={styles.cardRowIcon} />
							<span className={styles.cardRowText}>
								{formatAmount(item.totalAmount)} {item.currencyNamePL || ""}
							</span>
						</div>
						<div className={styles.cardRow}>
							<CalendarRegular className={styles.cardRowIcon} />
							<span className={styles.cardRowText}>
								{formatDateTime(item.createdAt)}
							</span>
						</div>
						{item.documentTypeNamePL && (
							<div className={styles.cardRow}>
								<PersonRegular className={styles.cardRowIcon} />
								<span className={styles.cardRowText}>
									{item.documentTypeNamePL}
								</span>
							</div>
						)}
					</div>
				</Card>
			))}
		</div>
	);

	return (
		<>
			{TableView}
			{CardView}
		</>
	);
};

export default CostsTable;
