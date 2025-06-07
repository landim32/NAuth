import { LanguageEnum } from './DTO/Enum/LanguageEnum';

export const getLangInfo = (lngCode: string | LanguageEnum) => {
  switch (lngCode) {
    case LanguageEnum.English:
    case 'en':
      return { code: 'en', flag: 'gb.svg', nameKey: 'language_english' };
    case LanguageEnum.Spanish:
    case 'es':
      return { code: 'es', flag: 'es.svg', nameKey: 'language_spanish' };
    case LanguageEnum.French:
    case 'fr':
      return { code: 'fr', flag: 'fr.svg', nameKey: 'language_french' };
    case LanguageEnum.Portuguese:
    case 'pt':
    case 'br':
      return { code: 'pt', flag: 'br.svg', nameKey: 'language_portuguese' };
    default:
      return { code: 'en', flag: 'gb.svg', nameKey: 'language_english' };
  }
};
