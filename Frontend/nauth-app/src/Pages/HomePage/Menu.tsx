import React, { useContext, useEffect, useState } from 'react';
import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/esm/Button';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import { Link, useNavigate } from 'react-router-dom';
import NavDropdown from 'react-bootstrap/NavDropdown';
import Alert from 'react-bootstrap/Alert';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faWarning } from '@fortawesome/free-solid-svg-icons/faWarning'
import { faBuilding, faCircleUser, faClose, faHome, faImage, faLock, faPencil, faSignInAlt, faUser } from '@fortawesome/free-solid-svg-icons';
import { useTranslation } from 'react-i18next';
import AuthContext from '../../NAuth/Contexts/Auth/AuthContext';
import MessageToast from '../../NAuth/Components/MessageToast';
import { MessageToastEnum } from '../../NAuth/DTO/Enum/MessageToastEnum';
import { ImageModal, ImageTypeEnum } from '../../NAuth/Components/ImageModal';
import { MenuLanguage } from '../../NAuth/Components/Functions';


export default function Menu() {

  const [showAlert, setShowAlert] = useState<boolean>(true);
  const [showImageModal, setShowImageModal] = useState<boolean>(false);

  const [showMessage, setShowMessage] = useState<boolean>(false);
  const [messageText, setMessageText] = useState<string>("");

  const { t } = useTranslation();

  const throwError = (message: string) => {
    setMessageText(message);
    setShowMessage(true);
  };

  let navigate = useNavigate();

  const authContext = useContext(AuthContext);

  useEffect(() => {
    authContext.loadUserSession().then((ret) => {
      if (!ret.sucesso) {
        throwError(ret.mensagemErro);
      }
    });
  }, []);
  return (
    <>
      <MessageToast
        dialog={MessageToastEnum.Error}
        showMessage={showMessage}
        messageText={messageText}
        onClose={() => setShowMessage(false)}
      ></MessageToast>
      <ImageModal
        Image={ImageTypeEnum.User}
        show={showImageModal}
        onClose={() => setShowImageModal(false)}
      />
      <Navbar expand="lg" className="navbar-dark bg-dark mb-3 border-bottom">
        <Container>
          <Link className='navbar-brand' to="/">{process.env.REACT_APP_PROJECT_NAME}</Link>
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="me-auto"> {/* Apply t() to link texts */}
              <Link className='nav-link' to="/"><FontAwesomeIcon icon={faHome} fixedWidth /> {t('home')}</Link>
              {!authContext.sessionInfo &&
                <Link className='nav-link' to="/new-seller"><FontAwesomeIcon icon={faUser} fixedWidth /> {t('be_a_representative')}</Link>
              }
              <Link className='nav-link' to="/network"><FontAwesomeIcon icon={faBuilding} fixedWidth /> {t('create_your_network')}</Link>
            </Nav>
          </Navbar.Collapse>
          <Navbar.Collapse>

            <Nav className="ms-auto justify-content-end">
              
              <MenuLanguage />
              {
                authContext.sessionInfo ?
                  <NavDropdown title={
                    <>
                      <FontAwesomeIcon icon={faCircleUser} />&nbsp;
                      <span>{authContext.sessionInfo.name}</span>
                    </>
                  } id="basic-nav-dropdown">
                    <NavDropdown.Item onClick={() => setShowImageModal(true)}>
                      <FontAwesomeIcon icon={faImage} fixedWidth /> {t('change_picture')}
                    </NavDropdown.Item>
                    <NavDropdown.Item onClick={async () => {
                      navigate("/account/edit-account");
                    }}><FontAwesomeIcon icon={faPencil} fixedWidth /> {t('edit_account')}</NavDropdown.Item>
                    <NavDropdown.Item onClick={async () => {
                      navigate("/account/change-password");
                    }}><FontAwesomeIcon icon={faLock} fixedWidth /> {t('change_password')}</NavDropdown.Item>
                    <NavDropdown.Divider />
                    <NavDropdown.Item onClick={async () => {
                      let ret = authContext.logout();
                      if (!ret.sucesso) {
                        throwError(ret.mensagemErro);
                      }
                      navigate(0);
                    }}><FontAwesomeIcon icon={faClose} fixedWidth /> {t('logout')}</NavDropdown.Item>
                  </NavDropdown>
                  :
                  <>
                    <Nav.Item>
                      <Button variant="danger" onClick={async () => {
                        navigate("/account/login");
                      }}> {/* Use t() for button text */}
                        <FontAwesomeIcon icon={faSignInAlt} fixedWidth /> {t('sign_in')}
                      </Button>
                    </Nav.Item>
                  </>
              }
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>
      {showAlert &&
        <Container>
          <Alert key="danger" variant="danger" onClose={() => setShowAlert(false)} dismissible>
            <FontAwesomeIcon icon={faWarning} /> {t('trial_version_warning')}
          </Alert>
        </Container>
      }
    </>
  );
}
