import { LanguageEnum } from '../DTO/Enum/LanguageEnum';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { useTranslation } from 'react-i18next';
import { getLangInfo } from '../../i18n';

const showFrequencyMin = (frequency: number, t: (key: string) => string) => {
  switch (frequency) {
    case 0:
      return t('frequency_unique');
    case 7:
      return t('frequency_week');
    case 30:
      return t('frequency_month');
    case 60:
      return t('frequency_bimonthly');
    case 90:
      return t('frequency_quarter');
    case 180:
      return t('frequency_half');
    case 365:
      return t('frequency_year');
    default:
      return String(frequency);
  }
};

const showFrequencyMax = (frequency: number, t: (key: string) => string) => {
  switch (frequency) {
    case 0:
      return t('frequency_unique_payment');
    case 7:
      return t('frequency_weekly_payment');
    case 30:
      return t('frequency_monthly_payment');
    case 60:
      return t('frequency_bimonthly_payment');
    case 90:
      return t('frequency_quarterly_payment');
    case 180:
      return t('frequency_semiannual_payment');
    case 365:
      return t('frequency_annual_payment');
    default:
      return String(frequency);
  }
};

function formatPhoneNumber(phone: string) {
  // Remove qualquer caractere que não seja número
  const digits = phone.replace(/\D/g, '');

  if (digits.length !== 11) {return phone;} // Retorna original se não tiver 11 dígitos

  const ddd = digits.slice(0, 2);
  const firstDigit = digits.slice(2, 3);
  const firstPart = digits.slice(3, 7);
  const secondPart = digits.slice(7);

  return (
    <>
      <small>({ddd})</small> {firstDigit} {firstPart}-{secondPart}
    </>
  );
}

const MenuLanguage = () => {
  const { i18n, t } = useTranslation();
  const currentLangInfo = getLangInfo(i18n.language);

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
  };

  const supportedLanguages = [
    getLangInfo(LanguageEnum.Portuguese), // Default/BR
    getLangInfo(LanguageEnum.English),
    getLangInfo(LanguageEnum.Spanish),
    getLangInfo(LanguageEnum.French),
  ];

  return (
    <NavDropdown
      title={
        <>
          <img
            src={`${import.meta.env.BASE_URL}flags/${currentLangInfo.flag}`}
            alt={t(currentLangInfo.nameKey)}
            style={{ width: '21px', height: '21px' }}
          />
        </>
      }
      id="basic-nav-dropdown">
      {supportedLanguages.map((lang) => (
        <NavDropdown.Item key={lang.code} onClick={() => changeLanguage(lang.code)}>
          <img src={`${import.meta.env.BASE_URL}flags/${lang.flag}`} alt={t(lang.nameKey)} style={{ width: '21px', height: '21px' }} />
          &nbsp;{t(lang.nameKey)}
        </NavDropdown.Item>
      ))}
    </NavDropdown>
  );
};

const langToStr = (lang: LanguageEnum) => {
  // Use the centralized getLangInfo from i18n.ts
  return getLangInfo(lang).code;
};

export { showFrequencyMin, showFrequencyMax, formatPhoneNumber, MenuLanguage, langToStr };
