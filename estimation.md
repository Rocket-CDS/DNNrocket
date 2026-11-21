# Estimation Technique – Site Internet CCABSV (DNN CMS / RocketCDS)

> Document de travail basé sur `requirements.md`. Les heures sont des estimations
> provisoires (`??`) à ajuster selon le volume de contenu réel et la réutilisation
> de modules RocketCDS existants (RocketContentAPI, RocketBlogAPI, RocketEventsAPI,
> RocketDirectoryAPI, RocketForms, RocketNewsAPI).

---

## A. Socle principal (livrable de base)

| # | Poste | Détail | Heures |
|---|-------|--------|--------|
| A1 | Installation / configuration DNN + RocketCDS | Setup environnement, portail, thème, modules de base | 12 |
| A2 | Intégration charte graphique | Déclinaison maquette (desktop + mobile) en thème DNN | 24 |
| A3 | Arborescence & gabarits de pages | 6 sections + accueil, templates RocketContentAPI | 20 |
| A4 | Page d'accueil | Accès rapides, actualités, agenda, moteur de recherche | 10 |
| A5 | Module Actualités | RocketNewsAPI : communiqués, infos urgentes | 6 |
| A6 | Module Agenda communautaire | RocketEventsAPI | 6 |
| A7 | Module Publications | Bulletins, guides, docs téléchargeables (RocketContentAPI) | 6 |
| A8 | Formulaires de contact | RocketForms + reCAPTCHA v3 | 8 |
| A9 | Accessibilité RGAA niveau AA | Audit + corrections thème/templates | 16 |
| A10 | Responsive mobile | Tests et ajustements multi-devices | 10 |
| A11 | Moteur de recherche interne | Config module recherche DNN/RocketCDS | 6 |
| A12 | RGPD | Mentions légales, confidentialité, CGU, bandeau consentement CNIL | 8 |
| A13 | Sécurité | HTTPS/SSL, durcissement, sauvegardes, PRA | 8 |
| A14 | Statistiques GA4 + Search Console | Intégration et configuration | 4 |
| A15 | Réseaux sociaux | Intégration flux/liens (Instagram, Facebook) | 3 |
| A16 | Espace contributeurs / intranet | Rôles, permissions DNN | 8 |
| A17 | SEO on-page | Métadonnées, sitemap, structure Hn | 6 |
| A18 | Recette, tests, corrections | Recette fonctionnelle + accessibilité | 12 |
| A19 | Mise en ligne | Bascule DNS, go-live | 4 |
| A20 | Formation contributeurs + tutoriels | Support écrit + session(s) | 8 |
| **Sous-total A** | | | **185 h** |

---

## B. Options (hors socle, à chiffrer séparément)

| # | Option | Détail | Heures |
|---|--------|--------|--------|
| B1 | Carte interactive du territoire | Intégration flux SITA (JSON), affichage dynamique | 20 |
| B2 | Widgets ADAT / SITA | Intégration widgets tiers (POI, hébergements, etc.) | 8 |
| B3 | Lecteur type Calaméo | Visionneuse de documents feuilletables | 6 |
| B4 | Intégration vidéos / sons | Lecteurs médias enrichis | 4 |
| B5 | Nouveaux noms de domaine | Achat/configuration DNS supplémentaires | 2 |
| B6 | Hébergement & sauvegardes (prestataire) | Offre hébergement géré + sauvegardes régulières | 4 |
| B7 | Maintenance évolutive (contrat annuel) | Forfait heures post-garantie | 20 |
| B8 | Garantie étendue (au-delà de 3-6 mois) | Prolongation période de garantie | 4 |
| **Sous-total B (options)** | | | **68 h** |

---

## C. Récapitulatif

| Bloc | Heures |
|------|--------|
| A. Socle principal | 185 h |
| B. Options sélectionnées | 68 h |
| **Total** | **253 h** |

---

## Notes

- Les heures marquées `??` doivent être complétées selon la vélocité de l'équipe
  et le niveau de réutilisation des modules RocketCDS existants (dépôts
  `RocketBlogAPI`, `RocketContentAPI`, `RocketDirectoryAPI`, `RocketEventsAPI`,
  `RocketForms`, `RocketNewsAPI`, `RocketModules`).
- Le volume de contenu (nombre de pages, documents, médias) à migrer/saisir
  impacte directement les postes A3, A4, A7.
- La carte interactive (B1) et les widgets ADAT/SITA (B2) dépendent du format
  exact des flux fournis par le SITA — à valider avant chiffrage ferme.
