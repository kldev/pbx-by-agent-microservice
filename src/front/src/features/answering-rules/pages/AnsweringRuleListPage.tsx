import {
	Button,
	Dropdown,
	Option,
	SearchBox,
	type SearchBoxChangeEvent,
	type SelectTabData,
	type SelectTabEvent,
	Tab,
	TabList,
	Text,
	makeStyles,
	tokens,
} from "@fluentui/react-components";
import {
	AddRegular,
	ChevronLeftRegular,
	ChevronRightRegular,
} from "@fluentui/react-icons";
import type React from "react";
import { useCallback, useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router";

import {
	useDeleteRule,
	useGetRulesList,
	useToggleRule,
} from "../../../api/answerrule/endpoints/answering-rules/answering-rules";
import {
	AnsweringRuleAction,
	type AnsweringRuleResponse,
} from "../../../api/answerrule/models";
import {
	ConfirmDialog,
	EmptyState,
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { useDebounce } from "../../../hooks/useDebounce";
import { ROUTES } from "../../../routes/routes";
import { AnsweringRuleTable } from "../components";

const STATUS_OPTIONS = [
	{ value: "all", label: "Wszystkie" },
	{ value: "true", label: "Aktywne" },
	{ value: "false", label: "Nieaktywne" },
];

const ACTION_TYPE_OPTIONS: { value: string; label: string }[] = [
	{ value: "all", label: "Wszystkie typy" },
	{ value: AnsweringRuleAction.Voicemail, label: "Poczta głosowa" },
	{ value: AnsweringRuleAction.Redirect, label: "Przekierowanie" },
	{
		value: AnsweringRuleAction.RedirectToGroup,
		label: "Przekierowanie do grupy",
	},
	{
		value: AnsweringRuleAction.DisconnectWithVoicemessage,
		label: "Komunikat głosowy",
	},
];

const useStyles = makeStyles({
	container: { display: "flex", flexDirection: "column", height: "100%" },
	tabsContainer: {
		paddingLeft: tokens.spacingHorizontalL,
		paddingRight: tokens.spacingHorizontalL,
		borderBottom: `1px solid ${tokens.colorNeutralStroke2}`,
	},
	tabList: { overflowX: "auto" },
	toolbar: {
		display: "flex",
		alignItems: "center",
		gap: tokens.spacingHorizontalM,
		padding: tokens.spacingHorizontalL,
		paddingTop: tokens.spacingVerticalM,
		paddingBottom: tokens.spacingVerticalM,
		flexWrap: "wrap",
	},
	searchBox: { minWidth: "280px" },
	content: {
		flex: 1,
		padding: tokens.spacingHorizontalL,
		paddingTop: 0,
		overflowX: "auto",
	},
	pagination: {
		display: "flex",
		alignItems: "center",
		justifyContent: "flex-end",
		gap: tokens.spacingHorizontalS,
		padding: tokens.spacingVerticalM,
		borderTop: `1px solid ${tokens.colorNeutralStroke2}`,
	},
	paginationInfo: { marginRight: tokens.spacingHorizontalM },
});

const DEFAULT_PAGE_SIZE = 20;
const SEARCH_DEBOUNCE_MS = 300;

const AnsweringRuleListPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const [searchParams, setSearchParams] = useSearchParams();

	const getInitialStatus = (): string => {
		const statusParam = searchParams.get("status");
		if (statusParam && STATUS_OPTIONS.some((o) => o.value === statusParam)) {
			return statusParam;
		}
		return "all";
	};

	const [searchValue, setSearchValue] = useState("");
	const [selectedStatus, setSelectedStatus] =
		useState<string>(getInitialStatus);
	const [selectedActionType, setSelectedActionType] = useState("all");
	const [pageNumber, setPageNumber] = useState(1);
	const debouncedSearch = useDebounce(searchValue, SEARCH_DEBOUNCE_MS);
	const [deleteItem, setDeleteItem] = useState<AnsweringRuleResponse | null>(
		null,
	);

	const isEnabledValue =
		selectedStatus === "all" ? undefined : selectedStatus === "true";
	const actionTypeValue =
		selectedActionType === "all"
			? undefined
			: (selectedActionType as AnsweringRuleAction);

	const rulesListMutation = useGetRulesList();
	const deleteRuleMutation = useDeleteRule();
	const toggleRuleMutation = useToggleRule();

	const fetchList = useCallback(() => {
		rulesListMutation.mutate({
			data: {
				pageNumber,
				pageSize: DEFAULT_PAGE_SIZE,
				search: debouncedSearch || undefined,
				isEnabled: isEnabledValue,
				actionType: actionTypeValue,
			},
		});
	}, [pageNumber, debouncedSearch, isEnabledValue, actionTypeValue]);

	// biome-ignore lint/correctness/useExhaustiveDependencies: fetch on filter change
	useEffect(() => {
		fetchList();
	}, [pageNumber, debouncedSearch, isEnabledValue, actionTypeValue]);

	const result = rulesListMutation.data;
	const items: AnsweringRuleResponse[] =
		result?.status === 200 ? (result.data.items ?? []) : [];
	const totalCount =
		result?.status === 200 ? (result.data.totalCount ?? 0) : 0;
	const totalPages =
		result?.status === 200 ? (result.data.totalPages ?? 1) : 1;
	const hasNextPage =
		result?.status === 200 ? (result.data.hasNextPage ?? false) : false;
	const hasPreviousPage =
		result?.status === 200 ? (result.data.hasPreviousPage ?? false) : false;
	const hasActiveFilters =
		searchValue || selectedStatus !== "all" || selectedActionType !== "all";

	const handleRowClick = (item: AnsweringRuleResponse) => {
		if (item.gid) navigate(`/answering-rules/${item.gid}`);
	};

	const handleShowDetails = (item: AnsweringRuleResponse) => {
		if (item.gid) navigate(`/answering-rules/${item.gid}`);
	};

	const handleEdit = (item: AnsweringRuleResponse) => {
		if (item.gid) navigate(`/answering-rules/${item.gid}/edit`);
	};

	const handleSearchChange = useCallback(
		(_ev: SearchBoxChangeEvent, data: { value: string }) => {
			setSearchValue(data.value);
			setPageNumber(1);
		},
		[],
	);

	const handleSearchClear = useCallback(() => {
		setSearchValue("");
		setPageNumber(1);
	}, []);

	const handleStatusTabSelect = useCallback(
		(_: SelectTabEvent, data: SelectTabData) => {
			const newStatus = data.value as string;
			setSelectedStatus(newStatus);
			setPageNumber(1);
			const newParams = new URLSearchParams(searchParams);
			if (newStatus === "all") {
				newParams.delete("status");
			} else {
				newParams.set("status", newStatus);
			}
			setSearchParams(newParams, { replace: true });
		},
		[searchParams, setSearchParams],
	);

	const handleDeleteClick = useCallback((item: AnsweringRuleResponse) => {
		setDeleteItem(item);
	}, []);

	const handleDeleteConfirm = useCallback(() => {
		if (!deleteItem?.gid) return;
		deleteRuleMutation.mutate(
			{ gid: deleteItem.gid },
			{
				onSuccess: () => {
					setDeleteItem(null);
					fetchList();
				},
			},
		);
	}, [deleteItem, deleteRuleMutation, fetchList]);

	const handleToggle = useCallback(
		(item: AnsweringRuleResponse) => {
			if (!item.gid) return;
			toggleRuleMutation.mutate(
				{ gid: item.gid },
				{ onSuccess: () => fetchList() },
			);
		},
		[toggleRuleMutation, fetchList],
	);

	return (
		<div className={styles.container}>
			<PageHeader
				title="Reguły odpowiadania"
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Reguły odpowiadania" },
				]}
				actions={
					<Button
						appearance="primary"
						icon={<AddRegular />}
						onClick={() => navigate(ROUTES.ANSWERING_RULES_CREATE)}
					>
						Nowa reguła
					</Button>
				}
			/>

			<div className={styles.tabsContainer}>
				<TabList
					className={styles.tabList}
					selectedValue={selectedStatus}
					onTabSelect={handleStatusTabSelect}
					size="small"
				>
					{STATUS_OPTIONS.map((o) => (
						<Tab key={o.value} value={o.value}>
							{o.label}
						</Tab>
					))}
				</TabList>
			</div>

			<div className={styles.toolbar}>
				<SearchBox
					className={styles.searchBox}
					placeholder="Szukaj po nazwie..."
					value={searchValue}
					onChange={handleSearchChange}
					dismiss={searchValue ? { onClick: handleSearchClear } : undefined}
				/>
				<Dropdown
					placeholder="Typ akcji"
					value={
						ACTION_TYPE_OPTIONS.find((o) => o.value === selectedActionType)
							?.label ?? "Wszystkie typy"
					}
					selectedOptions={[selectedActionType]}
					onOptionSelect={(_, data) => {
						setSelectedActionType(data.optionValue ?? "all");
						setPageNumber(1);
					}}
					style={{ minWidth: 200 }}
				>
					{ACTION_TYPE_OPTIONS.map((o) => (
						<Option key={o.value} value={o.value} text={o.label}>
							{o.label}
						</Option>
					))}
				</Dropdown>
			</div>

			<div className={styles.content}>
				{rulesListMutation.isPending && (
					<LoadingSpinner label="Ładowanie reguł..." />
				)}
				{rulesListMutation.isError && (
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać listy reguł."
						onRetry={fetchList}
					/>
				)}
				{rulesListMutation.isSuccess && items.length === 0 && (
					<EmptyState
						title={hasActiveFilters ? "Brak wyników" : "Brak reguł"}
						description={
							hasActiveFilters
								? "Nie znaleziono reguł pasujących do filtrów."
								: "Nie znaleziono żadnych reguł. Dodaj nową, aby rozpocząć."
						}
						actionLabel={hasActiveFilters ? undefined : "Dodaj pierwszą regułę"}
						onAction={
							hasActiveFilters
								? undefined
								: () => navigate(ROUTES.ANSWERING_RULES_CREATE)
						}
					/>
				)}
				{rulesListMutation.isSuccess && items.length > 0 && (
					<>
						<AnsweringRuleTable
							items={items}
							onRowClick={handleRowClick}
							onShowDetails={handleShowDetails}
							onEdit={handleEdit}
							onDelete={handleDeleteClick}
							onToggle={handleToggle}
						/>
						<div className={styles.pagination}>
							<Text className={styles.paginationInfo}>
								Strona {pageNumber} z {totalPages} (łącznie {totalCount}{" "}
								rekordów)
							</Text>
							<Button
								appearance="subtle"
								icon={<ChevronLeftRegular />}
								disabled={!hasPreviousPage}
								onClick={() => setPageNumber((p) => p - 1)}
							>
								Poprzednia
							</Button>
							<Button
								appearance="subtle"
								icon={<ChevronRightRegular />}
								iconPosition="after"
								disabled={!hasNextPage}
								onClick={() => setPageNumber((p) => p + 1)}
							>
								Następna
							</Button>
						</div>
					</>
				)}
			</div>

			{deleteItem && (
				<ConfirmDialog
					open={!!deleteItem}
					onOpenChange={(open) => !open && setDeleteItem(null)}
					title="Usuń regułę"
					message={`Czy na pewno chcesz usunąć regułę "${deleteItem.name || "bez nazwy"}"? Tej operacji nie można cofnąć.`}
					confirmLabel="Usuń"
					cancelLabel="Anuluj"
					onConfirm={handleDeleteConfirm}
					danger
					loading={deleteRuleMutation.isPending}
				/>
			)}
		</div>
	);
};

export default AnsweringRuleListPage;
