import {
	Button,
	makeStyles,
	SearchBox,
	type SearchBoxChangeEvent,
	type SelectTabData,
	type SelectTabEvent,
	Tab,
	TabList,
	Text,
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
	ConfirmDialog,
	EmptyState,
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import { ROUTES } from "../../../routes/routes";
import { TeamTable } from "../components";
import { useDebounce } from "../../../hooks/useDebounce";
import type {
	TeamListFilter,
	TeamResponse,
} from "../../../api/identity/models";
import {
	useDeleteTeam,
	useGetTeamList,
	useUpdateTeam,
} from "../../../api/identity/endpoints/teams/teams";

const STATUS_OPTIONS = [
	{ value: "all", label: "Wszystkie" },
	{ value: "true", label: "Aktywne" },
	{ value: "false", label: "Nieaktywne" },
];

const useStyles = makeStyles({
	container: { display: "flex", flexDirection: "column", height: "100%" },
	tabsContainer: {
		paddingLeft: tokens.spacingHorizontalL,
		paddingRight: tokens.spacingHorizontalL,
		borderBottom: `1px solid ${tokens.colorNeutralStroke2}`,
	},
	tabList: {
		overflowX: "auto",
	},
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

const TeamListPage: React.FC = () => {
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
	const [pageNumber, setPageNumber] = useState(1);
	const debouncedSearch = useDebounce(searchValue, SEARCH_DEBOUNCE_MS);
	const [deleteItem, setDeleteItem] = useState<TeamResponse | null>(null);

	const isActiveValue =
		selectedStatus === "all" ? undefined : selectedStatus === "true";

	const filter: TeamListFilter = {
		pageNumber: pageNumber,
		pageSize: DEFAULT_PAGE_SIZE,
		search: debouncedSearch || undefined,
		isActive: isActiveValue,
	};

	const teamListMutation = useGetTeamList();
	const deleteTeamMutation = useDeleteTeam();
	const updateTeamMutation = useUpdateTeam();

	useEffect(() => {
		teamListMutation.mutate({ data: filter });
	}, [pageNumber, debouncedSearch, isActiveValue]);

	const result = teamListMutation.data;
	const items: TeamResponse[] =
		result?.status === 200 ? (result.data.items ?? []) : [];
	const totalCount = result?.status === 200 ? (result.data.totalCount ?? 0) : 0;
	const totalPages = result?.status === 200 ? (result.data.totalPages ?? 1) : 1;
	const currentPage = pageNumber;
	const hasNextPage =
		result?.status === 200 ? (result.data.hasNextPage ?? false) : false;
	const hasPreviousPage =
		result?.status === 200 ? (result.data.hasPreviousPage ?? false) : false;
	const hasActiveFilters = searchValue || selectedStatus !== "all";

	const handleCreateNew = () => {
		navigate(ROUTES.TEAMS_CREATE);
	};

	const handleRowClick = (item: TeamResponse) => {
		if (item.gid) {
			navigate(`/teams/${item.gid}`);
		}
	};

	const handleShowDetails = (item: TeamResponse) => {
		if (item.gid) {
			navigate(`/teams/${item.gid}`);
		}
	};

	const handleEdit = (item: TeamResponse) => {
		if (item.gid) {
			navigate(`/teams/${item.gid}/edit`);
		}
	};

	const handlePreviousPage = () => {
		if (hasPreviousPage) setPageNumber((prev) => prev - 1);
	};

	const handleNextPage = () => {
		if (hasNextPage) setPageNumber((prev) => prev + 1);
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

	const handleDeleteClick = useCallback((item: TeamResponse) => {
		setDeleteItem(item);
	}, []);

	const handleDeleteConfirm = useCallback(() => {
		if (!deleteItem?.gid) return;
		deleteTeamMutation.mutate(
			{ gid: deleteItem.gid },
			{
				onSuccess: () => {
					setDeleteItem(null);
					teamListMutation.mutate({ data: filter });
				},
			},
		);
	}, [deleteItem, deleteTeamMutation, teamListMutation, filter]);

	const handleToggleActive = useCallback(
		(item: TeamResponse) => {
			if (!item.gid) return;
			updateTeamMutation.mutate(
				{
					gid: item.gid,
					data: {
						code: item.code ?? "",
						name: item.name ?? "",
						structureId: item.structureId,
						isActive: !item.isActive,
					},
				},
				{
					onSuccess: () => {
						teamListMutation.mutate({ data: filter });
					},
				},
			);
		},
		[updateTeamMutation, teamListMutation, filter],
	);

	return (
		<div className={styles.container}>
			<PageHeader
				title="Zespoły"
				breadcrumbs={[{ label: "Dashboard", href: "/" }, { label: "Zespoły" }]}
				actions={
					<Button
						appearance="primary"
						icon={<AddRegular />}
						onClick={handleCreateNew}
					>
						Nowy zespół
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
					placeholder="Szukaj po nazwie lub kodzie..."
					value={searchValue}
					onChange={handleSearchChange}
					dismiss={searchValue ? { onClick: handleSearchClear } : undefined}
				/>
			</div>
			<div className={styles.content}>
				{teamListMutation.isPending && (
					<LoadingSpinner label="Ładowanie listy zespołów..." />
				)}
				{teamListMutation.isError && (
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać listy zespołów."
						onRetry={() => teamListMutation.mutate({ data: filter })}
					/>
				)}
				{teamListMutation.isSuccess && items.length === 0 && (
					<EmptyState
						title={hasActiveFilters ? "Brak wyników" : "Brak zespołów"}
						description={
							hasActiveFilters
								? "Nie znaleziono zespołów pasujących do filtrów."
								: "Nie znaleziono żadnych zespołów. Dodaj nowy, aby rozpocząć."
						}
						actionLabel={hasActiveFilters ? undefined : "Dodaj pierwszy zespół"}
						onAction={hasActiveFilters ? undefined : handleCreateNew}
					/>
				)}
				{teamListMutation.isSuccess && items.length > 0 && (
					<>
						<TeamTable
							items={items}
							onRowClick={handleRowClick}
							onShowDetails={handleShowDetails}
							onEdit={handleEdit}
							onDelete={handleDeleteClick}
							onToggleActive={handleToggleActive}
						/>
						<div className={styles.pagination}>
							<Text className={styles.paginationInfo}>
								Strona {currentPage} z {totalPages} (łącznie {totalCount}{" "}
								rekordów)
							</Text>
							<Button
								appearance="subtle"
								icon={<ChevronLeftRegular />}
								disabled={!hasPreviousPage}
								onClick={handlePreviousPage}
							>
								Poprzednia
							</Button>
							<Button
								appearance="subtle"
								icon={<ChevronRightRegular />}
								iconPosition="after"
								disabled={!hasNextPage}
								onClick={handleNextPage}
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
					title="Usuń zespół"
					message={`Czy na pewno chcesz usunąć zespół "${deleteItem.name || "bez nazwy"}"? Tej operacji nie można cofnąć.`}
					confirmLabel="Usuń"
					cancelLabel="Anuluj"
					onConfirm={handleDeleteConfirm}
					danger
					loading={deleteTeamMutation.isPending}
				/>
			)}
		</div>
	);
};

export default TeamListPage;
