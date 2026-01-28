import type { AppUserResponse } from "../../../api/identity/models";

/**
 * Interfejs Command Pattern dla dialogu zmiany zespołu.
 * Pozwala na imperatywne otwieranie dialogu z różnych miejsc aplikacji.
 */
export interface ChangeTeamDialogCommand {
	/**
	 * Otwórz dialog zmiany zespołu dla użytkownika.
	 * @param user - dane użytkownika do zmiany zespołu
	 */
	open: (user: AppUserResponse) => void;
}
