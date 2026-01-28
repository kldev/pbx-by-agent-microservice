import {
	Button,
	Dropdown,
	makeStyles,
	Option,
	type OptionOnSelectData,
	SearchBox,
	type SearchBoxChangeEvent,
	type SelectionEvents,
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
import { useCallback, useEffect, useRef, useState } from "react";
import { useNavigate, useSearchParams } from "react-router";
import {
	useDeleteAppUser,
	useGetAppUserList,
	useUpdateAppUser,
} from "../../../api/identity/endpoints/app-users/app-users";
import {
	type AppUserListFilter,
	type AppUserResponse,
	Department,
} from "../../../api/identity/models";
import {
	ConfirmDialog,
	EmptyState,
	ErrorMessage,
	LoadingSpinner,
	PageHeader,
} from "../../../components/common";
import {
	ChangeTeamDialog,
	SystemUserFormPanel,
	SystemUserTable,
} from "../components";
import type { ChangeTeamDialogCommand } from "../components/ChangeTeamDialogCommand";
import type { SystemUserFormCommand } from "../components/SystemUserFormCommand";

const ACTIVE_OPTIONS = [
	{ value: "all", label: "Wszystkie" },
	{ value: "true", label: "Aktywni" },
	{ value: "false", label: "Nieaktywni" },
];

const DEPARTMENT_OPTIONS = [
	{ value: "all", label: "Wszystkie działy" },
	{ value: Department.Operations, label: "Operacje" },
	{ value: Department.Finance, label: "Finanse" },
	{ value: Department.Developers, label: "IT" },
	{ value: Department.Support, label: "Support" },
	{ value: Department.Others, label: "Others" },
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
	dropdown: { minWidth: "150px" },
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

function useDebounce<T>(value: T, delay: number): T {
	const [debouncedValue, setDebouncedValue] = useState<T>(value);
	useEffect(() => {
		const handler = setTimeout(() => {
			setDebouncedValue(value);
		}, delay);
		return () => {
			clearTimeout(handler);
		};
	}, [value, delay]);
	return debouncedValue;
}

const SystemUserListPage: React.FC = () => {
	const styles = useStyles();
	const navigate = useNavigate();
	const [searchParams, setSearchParams] = useSearchParams();
	const systemUserFormRef = useRef<SystemUserFormCommand>(null);
	const changeTeamDialogRef = useRef<ChangeTeamDialogCommand>(null);

	const getInitialDepartment = (): string => {
		const deptParam = searchParams.get("department");
		if (deptParam && DEPARTMENT_OPTIONS.some((o) => o.value === deptParam)) {
			return deptParam;
		}
		return "all";
	};

	const [searchValue, setSearchValue] = useState("");
	const [selectedActive, setSelectedActive] = useState<string>("all");
	const [selectedDepartment, setSelectedDepartment] =
		useState<string>(getInitialDepartment);
	const debouncedSearch = useDebounce(searchValue, SEARCH_DEBOUNCE_MS);
	const [filter, setFilter] = useState<AppUserListFilter>({
		pageNumber: 1,
		pageSize: DEFAULT_PAGE_SIZE,
	});
	const [deleteItem, setDeleteItem] = useState<AppUserResponse | null>(null);

	const getAppUserListMutation = useGetAppUserList();
	const deleteAppUserMutation = useDeleteAppUser();
	const updateAppUserMutation = useUpdateAppUser();

	const refreshList = useCallback(() => {
		getAppUserListMutation.mutate({ data: filter });
	}, [filter, getAppUserListMutation]);

	useEffect(() => {
		const isActiveValue =
			selectedActive === "all" ? undefined : selectedActive === "true";

		const departmentValue =
			selectedDepartment === "all"
				? undefined
				: (selectedDepartment as (typeof Department)[keyof typeof Department]);

		setFilter((prev) => ({
			...prev,
			search: debouncedSearch || undefined,
			isActive: isActiveValue,
			department: departmentValue,
			pageNumber: 1,
		}));
	}, [debouncedSearch, selectedActive, selectedDepartment]);

	useEffect(() => {
		getAppUserListMutation.mutate({ data: filter });
	}, [filter]);

	const result = getAppUserListMutation.data;
	const items: AppUserResponse[] =
		result?.status === 200 ? (result.data.items ?? []) : [];
	const totalCount = result?.status === 200 ? (result.data.totalCount ?? 0) : 0;
	const totalPages = result?.status === 200 ? (result.data.totalPages ?? 1) : 1;
	const currentPage = filter.pageNumber ?? 1;
	const hasNextPage =
		result?.status === 200 ? (result.data.hasNextPage ?? false) : false;
	const hasPreviousPage =
		result?.status === 200 ? (result.data.hasPreviousPage ?? false) : false;
	const hasActiveFilters =
		searchValue || selectedActive !== "all" || selectedDepartment !== "all";

	const handleCreateNew = () => {
		systemUserFormRef.current?.create();
	};

	const handleRowClick = (item: AppUserResponse) => {
		if (item.gid) {
			navigate(`/system-users/${item.gid}`);
		}
	};

	const handleShowDetails = (item: AppUserResponse) => {
		if (item.gid) {
			navigate(`/system-users/${item.gid}`);
		}
	};

	const handleEdit = (item: AppUserResponse) => {
		if (item.gid) {
			systemUserFormRef.current?.edit(item.gid);
		}
	};

	const handleChangePassword = (item: AppUserResponse) => {
		if (item.gid) {
			navigate(`/system-users/${item.gid}?changePassword=true`);
		}
	};

	const handleChangeTeam = useCallback((item: AppUserResponse) => {
		changeTeamDialogRef.current?.open(item);
	}, []);

	const handlePreviousPage = () => {
		if (hasPreviousPage)
			setFilter((prev) => ({
				...prev,
				pageNumber: (prev.pageNumber ?? 1) - 1,
			}));
	};

	const handleNextPage = () => {
		if (hasNextPage)
			setFilter((prev) => ({
				...prev,
				pageNumber: (prev.pageNumber ?? 1) + 1,
			}));
	};

	const handleSearchChange = useCallback(
		(_ev: SearchBoxChangeEvent, data: { value: string }) => {
			setSearchValue(data.value);
		},
		[],
	);

	const handleSearchClear = useCallback(() => {
		setSearchValue("");
	}, []);

	const handleActiveFilterChange = useCallback(
		(_ev: SelectionEvents, data: OptionOnSelectData) => {
			setSelectedActive(data.optionValue as string);
		},
		[],
	);

	const handleDepartmentTabSelect = useCallback(
		(_: SelectTabEvent, data: SelectTabData) => {
			const newDept = data.value as string;
			setSelectedDepartment(newDept);
			const newParams = new URLSearchParams(searchParams);
			if (newDept === "all") {
				newParams.delete("department");
			} else {
				newParams.set("department", newDept);
			}
			setSearchParams(newParams, { replace: true });
		},
		[searchParams, setSearchParams],
	);

	const handleDeleteClick = useCallback((item: AppUserResponse) => {
		setDeleteItem(item);
	}, []);

	const handleDeleteConfirm = useCallback(() => {
		if (!deleteItem?.gid) return;
		deleteAppUserMutation.mutate(
			{ gid: deleteItem.gid },
			{
				onSuccess: () => {
					setDeleteItem(null);
					getAppUserListMutation.mutate({ data: filter });
				},
			},
		);
	}, [deleteItem, deleteAppUserMutation, filter, getAppUserListMutation]);

	const handleToggleActive = useCallback(
		(item: AppUserResponse) => {
			if (!item.gid) return;
			updateAppUserMutation.mutate(
				{
					gid: item.gid,
					data: {
						firstName: item.firstName ?? "",
						lastName: item.lastName ?? "",
						email: item.email ?? "",
						isActive: !item.isActive,
					},
				},
				{
					onSuccess: () => {
						getAppUserListMutation.mutate({ data: filter });
					},
				},
			);
		},
		[updateAppUserMutation, filter, getAppUserListMutation],
	);

	const selectedActiveLabel = ACTIVE_OPTIONS.find(
		(opt) => opt.value === selectedActive,
	)?.label;

	const getFullName = (item: AppUserResponse) => {
		const parts = [item.firstName, item.lastName].filter(Boolean);
		return parts.length > 0 ? parts.join(" ") : "bez nazwy";
	};

	return (
		<div className={styles.container}>
			<PageHeader
				title="Użytkownicy systemu"
				breadcrumbs={[
					{ label: "Dashboard", href: "/" },
					{ label: "Użytkownicy systemu" },
				]}
				actions={
					<Button
						appearance="primary"
						icon={<AddRegular />}
						onClick={handleCreateNew}
					>
						Nowy użytkownik
					</Button>
				}
			/>
			<div className={styles.tabsContainer}>
				<TabList
					className={styles.tabList}
					selectedValue={selectedDepartment}
					onTabSelect={handleDepartmentTabSelect}
					size="small"
				>
					{DEPARTMENT_OPTIONS.map((o) => (
						<Tab key={o.value} value={o.value}>
							{o.label}
						</Tab>
					))}
				</TabList>
			</div>
			<div className={styles.toolbar}>
				<SearchBox
					className={styles.searchBox}
					placeholder="Szukaj po imieniu, nazwisku lub email..."
					value={searchValue}
					onChange={handleSearchChange}
					dismiss={searchValue ? { onClick: handleSearchClear } : undefined}
				/>
				<Dropdown
					className={styles.dropdown}
					placeholder="Status"
					value={selectedActiveLabel}
					selectedOptions={[selectedActive]}
					onOptionSelect={handleActiveFilterChange}
				>
					{ACTIVE_OPTIONS.map((option) => (
						<Option key={option.value} value={option.value}>
							{option.label}
						</Option>
					))}
				</Dropdown>
			</div>
			<div className={styles.content}>
				{getAppUserListMutation.isPending && (
					<LoadingSpinner label="Ładowanie listy użytkowników..." />
				)}
				{getAppUserListMutation.isError && (
					<ErrorMessage
						title="Błąd ładowania"
						message="Nie udało się pobrać listy użytkowników."
						onRetry={() => getAppUserListMutation.mutate({ data: filter })}
					/>
				)}
				{getAppUserListMutation.isSuccess && items.length === 0 && (
					<EmptyState
						title={hasActiveFilters ? "Brak wyników" : "Brak użytkowników"}
						description={
							hasActiveFilters
								? "Nie znaleziono użytkowników pasujących do filtrów."
								: "Nie znaleziono żadnych użytkowników. Dodaj nowego, aby rozpocząć."
						}
						actionLabel={
							hasActiveFilters ? undefined : "Dodaj pierwszego użytkownika"
						}
						onAction={hasActiveFilters ? undefined : handleCreateNew}
					/>
				)}
				{getAppUserListMutation.isSuccess && items.length > 0 && (
					<>
						<SystemUserTable
							items={items}
							onRowClick={handleRowClick}
							onShowDetails={handleShowDetails}
							onEdit={handleEdit}
							onDelete={handleDeleteClick}
							onToggleActive={handleToggleActive}
							onChangePassword={handleChangePassword}
							onChangeTeam={handleChangeTeam}
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
					title="Usuń użytkownika"
					message={`Czy na pewno chcesz usunąć użytkownika "${getFullName(deleteItem)}"? Tej operacji nie można cofnąć.`}
					confirmLabel="Usuń"
					cancelLabel="Anuluj"
					onConfirm={handleDeleteConfirm}
					danger
					loading={deleteAppUserMutation.isPending}
				/>
			)}

			<SystemUserFormPanel ref={systemUserFormRef} onSuccess={refreshList} />

			<ChangeTeamDialog ref={changeTeamDialogRef} onSuccess={refreshList} />
		</div>
	);
};

export default SystemUserListPage;
