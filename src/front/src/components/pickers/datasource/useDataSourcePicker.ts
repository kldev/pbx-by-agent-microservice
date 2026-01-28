import { useCallback } from "react";
import {
	getSuggestions,
	searchData,
	resolveData,
} from "../../../api/datasource/endpoints/data-source/data-source";
import type {
	DataSourceType,
	PickerDataItem,
} from "../../../api/datasource/models";
import type { TagPickerOption } from "../TagPicker";

export interface UseDataSourcePickerOptions {
	dataSourceType: DataSourceType;
	contextFilters?: Record<string, string>;
	useGid?: boolean;
}

function mapToTagPickerOption(
	item: PickerDataItem,
	useGid: boolean = true,
): TagPickerOption {
	return {
		id: useGid ? item.gid || "" : item.recordId ? String(item.recordId) : "",
		name: item.label || "",
		secondary: item.subLabel || undefined,
		recordId: item.recordId || undefined,
		gid: item.gid || "",
	};
}

export function useDataSourcePicker({
	dataSourceType,
	contextFilters,
	useGid = true,
}: UseDataSourcePickerOptions) {
	const loadOptions = useCallback(
		async (
			query: string,
			ids?: string[],
			limit?: number,
		): Promise<TagPickerOption[]> => {
			try {
				// Resolve mode - when we have IDs to resolve
				if (ids && ids.length > 0) {
					const response = await resolveData({
						dataSourceType,
						gids: useGid ? ids : undefined,
						ids: useGid ? undefined : ids.map((id) => Number(id)),
					});
					return (response.data.items || []).map((item) =>
						mapToTagPickerOption(item, useGid),
					);
				}

				// Search mode - when query is provided
				if (query && query.trim().length >= 2) {
					const response = await searchData({
						dataSourceType,
						query: query.trim(),
						limit: limit || 25,
					});
					return (response.data.items || []).map((item) =>
						mapToTagPickerOption(item, useGid),
					);
				}

				// Suggestions mode - default when no query
				const response = await getSuggestions({
					dataSourceType,
					limit: limit || 25,
					contextFilters: contextFilters || undefined,
				});
				return (response.data.items || []).map((item) =>
					mapToTagPickerOption(item, useGid),
				);
			} catch (error) {
				console.error("DataSourcePicker load error:", error);
				return [];
			}
		},
		[dataSourceType, contextFilters, useGid],
	);

	return { loadOptions };
}

export default useDataSourcePicker;
