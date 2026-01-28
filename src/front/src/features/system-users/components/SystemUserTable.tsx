import {
	Avatar,
	Button,
	Card,
	CardHeader,
	Menu,
	MenuItem,
	MenuList,
	MenuPopover,
	MenuTrigger,
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
	DeleteRegular,
	EditRegular,
	EyeRegular,
	KeyRegular,
	MailRegular,
	MoreVerticalRegular,
	PeopleTeamRegular,
	ToggleLeftRegular,
} from "@fluentui/react-icons";
import type React from "react";
import type { AppUserResponse } from "../../../api/identity/models";
import SystemUserDepartmentBadge from "./SystemUserDepartmentBadge";
import SystemUserStatusBadge from "./SystemUserStatusBadge";

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
	actionsCell: {
		width: "48px",
	},
	avatarCell: {
		width: "200px",
	},
	avatarContainer: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalS,
	},
	emailCell: {
		width: "220px",
	},
	departmentCell: {
		width: "130px",
	},
	statusCell: {
		width: "100px",
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
	card: {
		cursor: "pointer",
		":hover": {
			backgroundColor: tokens.colorNeutralBackground1Hover,
		},
	},
	cardHeader: {
		paddingBottom: tokens.spacingVerticalXS,
	},
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

export interface SystemUserTableProps {
	items: AppUserResponse[];
	onRowClick?: (item: AppUserResponse) => void;
	onEdit?: (item: AppUserResponse) => void;
	onDelete?: (item: AppUserResponse) => void;
	onToggleActive?: (item: AppUserResponse) => void;
	onShowDetails?: (item: AppUserResponse) => void;
	onChangePassword?: (item: AppUserResponse) => void;
	onChangeTeam?: (item: AppUserResponse) => void;
}

function getInitials(
	firstName?: string | null,
	lastName?: string | null,
): string {
	const first = firstName?.[0]?.toUpperCase() || "";
	const last = lastName?.[0]?.toUpperCase() || "";
	return first + last || "?";
}

function getFullName(item: AppUserResponse): string {
	const parts = [item.firstName, item.lastName].filter(Boolean);
	return parts.length > 0 ? parts.join(" ") : "Brak nazwy";
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

const SystemUserTable: React.FC<SystemUserTableProps> = ({
	items,
	onRowClick,
	onEdit,
	onDelete,
	onToggleActive,
	onShowDetails,
	onChangePassword,
	onChangeTeam,
}) => {
	const styles = useStyles();

	const handleActionClick = (e: React.MouseEvent) => {
		e.stopPropagation();
	};

	const handleShowDetails = (item: AppUserResponse) => {
		if (onShowDetails) {
			onShowDetails(item);
		} else if (onRowClick) {
			onRowClick(item);
		}
	};

	const TableView = (
		<div className={styles.tableContainer}>
			<Table aria-label="Lista użytkowników" className={styles.table}>
				<TableHeader>
					<TableRow className={styles.headerRow}>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.actionsCell)}
						/>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.avatarCell)}
						>
							Użytkownik
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.emailCell)}
						>
							Email
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.departmentCell)}
						>
							Dział
						</TableHeaderCell>
						<TableHeaderCell
							className={mergeClasses(styles.headerCell, styles.statusCell)}
						>
							Status
						</TableHeaderCell>
					</TableRow>
				</TableHeader>
				<TableBody>
					{items.map((item, index) => (
						<TableRow
							key={item.gid}
							onClick={() => onRowClick?.(item)}
							className={index % 2 === 1 ? styles.rowEven : styles.row}
						>
							<TableCell
								className={styles.actionsCell}
								onClick={handleActionClick}
							>
								<Menu>
									<MenuTrigger disableButtonEnhancement>
										<Button
											appearance="subtle"
											icon={<MoreVerticalRegular />}
											aria-label="Akcje"
										/>
									</MenuTrigger>
									<MenuPopover>
										<MenuList>
											<MenuItem
												icon={<EyeRegular />}
												onClick={() => handleShowDetails(item)}
											>
												Szczegóły
											</MenuItem>
											<MenuItem
												icon={<EditRegular />}
												onClick={() => onEdit?.(item)}
											>
												Edytuj
											</MenuItem>
											<MenuItem
												icon={<KeyRegular />}
												onClick={() => onChangePassword?.(item)}
											>
												Zmień hasło
											</MenuItem>
											<MenuItem
												icon={<PeopleTeamRegular />}
												onClick={() => onChangeTeam?.(item)}
											>
												Zmień zespół
											</MenuItem>
											<MenuItem
												icon={<ToggleLeftRegular />}
												onClick={() => onToggleActive?.(item)}
											>
												{item.isActive ? "Dezaktywuj" : "Aktywuj"}
											</MenuItem>
											<MenuItem
												icon={<DeleteRegular />}
												onClick={() => onDelete?.(item)}
											>
												Usuń
											</MenuItem>
										</MenuList>
									</MenuPopover>
								</Menu>
							</TableCell>
							<TableCell className={styles.avatarCell}>
								<div className={styles.avatarContainer}>
									<Avatar
										name={getFullName(item)}
										initials={getInitials(item.firstName, item.lastName)}
										size={28}
									/>
									<TruncatedCell value={getFullName(item)} />
								</div>
							</TableCell>
							<TableCell className={styles.emailCell}>
								<TruncatedCell value={item.email} />
							</TableCell>
							<TableCell className={styles.departmentCell}>
								<SystemUserDepartmentBadge department={item.department} />
							</TableCell>
							<TableCell className={styles.statusCell}>
								<SystemUserStatusBadge isActive={item.isActive ?? false} />
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
				<Card
					key={item.gid}
					className={styles.card}
					onClick={() => onRowClick?.(item)}
				>
					<CardHeader
						className={styles.cardHeader}
						image={
							<Avatar
								name={getFullName(item)}
								initials={getInitials(item.firstName, item.lastName)}
								size={40}
							/>
						}
						header={<Text weight="semibold">{getFullName(item)}</Text>}
						description={
							<div
								style={{
									display: "flex",
									alignItems: "center",
									gap: "8px",
									marginTop: "4px",
								}}
							>
								<SystemUserStatusBadge isActive={item.isActive ?? false} />
								<SystemUserDepartmentBadge department={item.department} />
							</div>
						}
						action={
							<div onClick={handleActionClick}>
								<Menu>
									<MenuTrigger disableButtonEnhancement>
										<Button
											appearance="subtle"
											icon={<MoreVerticalRegular />}
											aria-label="Akcje"
											size="small"
										/>
									</MenuTrigger>
									<MenuPopover>
										<MenuList>
											<MenuItem
												icon={<EyeRegular />}
												onClick={() => handleShowDetails(item)}
											>
												Szczegóły
											</MenuItem>
											<MenuItem
												icon={<EditRegular />}
												onClick={() => onEdit?.(item)}
											>
												Edytuj
											</MenuItem>
											<MenuItem
												icon={<KeyRegular />}
												onClick={() => onChangePassword?.(item)}
											>
												Zmień hasło
											</MenuItem>
											<MenuItem
												icon={<PeopleTeamRegular />}
												onClick={() => onChangeTeam?.(item)}
											>
												Zmień zespół
											</MenuItem>
											<MenuItem
												icon={<ToggleLeftRegular />}
												onClick={() => onToggleActive?.(item)}
											>
												{item.isActive ? "Dezaktywuj" : "Aktywuj"}
											</MenuItem>
											<MenuItem
												icon={<DeleteRegular />}
												onClick={() => onDelete?.(item)}
											>
												Usuń
											</MenuItem>
										</MenuList>
									</MenuPopover>
								</Menu>
							</div>
						}
					/>
					<div className={styles.cardContent}>
						{item.email && (
							<div className={styles.cardRow}>
								<MailRegular className={styles.cardRowIcon} />
								<span className={styles.cardRowText}>{item.email}</span>
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

export default SystemUserTable;
