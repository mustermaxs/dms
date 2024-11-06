import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { ServiceLocator } from './serviceLocator';
import { ITagService, MockTagService, TagService } from './services/tagService';
import { DocumentService, IDocumentService, MockDocumentService } from './services/documentService';

const config = require("./config.json");
const isDebugMode = config.debug;

if (isDebugMode === "true") {
  ServiceLocator.register<ITagService, MockTagService>('ITagService', MockTagService);
  ServiceLocator.register<IDocumentService, MockDocumentService>('IDocumentService', MockDocumentService);
}
else {
  ServiceLocator.register<ITagService, TagService>('ITagService', TagService);
  ServiceLocator.register<IDocumentService, DocumentService>('IDocumentService', DocumentService);
}



const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
