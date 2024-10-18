import * as React from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import { ArrowDownTrayIcon, EyeIcon } from '@heroicons/react/24/solid'

import { ActionIcon, Badge } from 'rizzui'

interface RowData {
  id: string;
  title: string;
  match: string;
  tags: string[];
  content: string;
}

function createData(
  id: string,
  title: string,
  match: string,
  tags: string[],
  content: string
): RowData {
  return { id, title, match, tags, content };
}

const rows = [
  createData('1', 'Document 1.pdf', 'Lorem ipsum', ['tag1', 'tag2'], 'This is the content of Document 1.pdf'),
  createData('2', 'Document 2', 'Dolor sit amet', ['tag2', 'tag3'], 'This is the content of Document 2'),
  createData('3', 'Document 3', 'Consectetur adipiscing', ['tag1', 'tag3'], 'This is the content of Document 3'),
];

interface DocumentTableProps {
  openDocumentModal: (document: RowData) => void;
}

export default function DocumentTable({ openDocumentModal }: DocumentTableProps) {
  return (
    <TableContainer component={Paper}>
      <Table sx={{ minWidth: 650 }} aria-label="document table">
        <TableHead>
          <TableRow>
            <TableCell></TableCell>
            <TableCell>Title</TableCell>
            <TableCell>Match</TableCell>
            <TableCell>Tags</TableCell>
            <TableCell></TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {rows.map((row) => (
            <TableRow key={row.id}>
              <TableCell>
                <ActionIcon variant="outline" rounded="md" onClick={() => openDocumentModal(row)}>
                  <EyeIcon className="w-5 h-5" />
                </ActionIcon>
              </TableCell>
              <TableCell component="th" scope="row">
                <span className="font-bold" >{row.title}</span>
              </TableCell>
              <TableCell>{row.match}</TableCell>
              <TableCell>
                {row.tags.map((tag) => (
                  <Badge variant="outline" className='mr-1 hover:bg-black hover:text-white cursor-default' key={tag}>
                    {tag}
                  </Badge>
                ))}
              </TableCell>
              <TableCell>
                <ActionIcon variant="outline" rounded="lg" onClick={() => {}}>
                  <ArrowDownTrayIcon className="w-5 h-5" />
                </ActionIcon>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
