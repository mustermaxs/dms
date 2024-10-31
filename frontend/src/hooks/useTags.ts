import { useState, useEffect } from "react";
import { Tag } from "../types/Tag";
import { ServiceLocator } from "../serviceLocator";
import { MockTagService as ITagService } from "../services/tagService";

export const useTags = () => {
  const [availableTags, setAvailableTags] = useState<Tag[]>([]);
  const [isLoadingTags, setIsLoadingTags] = useState(false);

  useEffect(() => {
    const fetchTags = async () => {
      const tagService = ServiceLocator.resolve<ITagService>('ITagService');
      const tags = await tagService.getTags();
      setAvailableTags(tags);
      setIsLoadingTags(false);
    };

    fetchTags();
  }, [isLoadingTags]);

  return { availableTags, setAvailableTags, isLoadingTags, setIsLoadingTags };
};
