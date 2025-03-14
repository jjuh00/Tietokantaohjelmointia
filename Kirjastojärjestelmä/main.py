import tkinter as tk
from library_app import LibraryApp

def main():
    # Alustetaan root-ikkuna
    root = tk.Tk()

    # Määritellään tietokanta
    db_config = {"host": "localhost", "user": "root", "password": "root", "database": "LibraryDB"}

    # Alustetaan ja ajetaan ohjelma
    LibraryApp(root, db_config)

    # Käynnistetään ohjelma
    root.mainloop()

if __name__ == "__main__":
    main()