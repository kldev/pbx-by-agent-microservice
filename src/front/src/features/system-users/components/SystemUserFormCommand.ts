/**
 * Interfejs Command Pattern dla formularza użytkowników systemu.
 * Pozwala na imperatywne otwieranie formularza z różnych miejsc aplikacji.
 */
export interface SystemUserFormCommand {
	/**
	 * Otwórz formularz tworzenia nowego użytkownika.
	 */
	create: () => void;

	/**
	 * Otwórz formularz edycji istniejącego użytkownika.
	 * @param gid - identyfikator użytkownika do edycji
	 */
	edit: (gid: string) => void;
}
