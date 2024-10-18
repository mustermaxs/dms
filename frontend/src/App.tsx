import Header from "./components/shared/header";
import SearchInput from "./components/ui/searchInput";
import Container from "./components/shared/container";
import DocumentTable from './components/ui/DocumentTable';
import { useState } from "react";


function App() {

  const [isSearching, setIsSearching] = useState<boolean>(false);


  const handleSearch = (searchData: { tags: string[], content: string }) => {

    setTimeout(() => {
      console.log("searchData");
      console.table(searchData);
      setIsSearching(false);
    }, 500);
  };

  return (
    <Container>
      <Header />
      <SearchInput handleSearch={handleSearch} isSearching={isSearching} setIsSearching={setIsSearching} />
      <DocumentTable />
    </Container>
  );
}

export default App;
